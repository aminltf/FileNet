using System.Diagnostics;
using FileNet.WebFramework.Contexts;
using FileNet.WebFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileNet.WebFramework.ScanIngest;

public sealed class ScanIngestHostedService : BackgroundService
{
    private readonly ILogger<ScanIngestHostedService> _logger;
    private readonly ScanIngestOptions _opt;
    private readonly ScanFileNameParser _parser;
    private readonly IServiceScopeFactory _scopeFactory;

    public ScanIngestHostedService(
        IOptions<ScanIngestOptions> opt,
        ScanFileNameParser parser,
        IServiceScopeFactory scopeFactory,
        ILogger<ScanIngestHostedService> logger)
    {
        _logger = logger;
        _parser = parser;
        _scopeFactory = scopeFactory;
        _opt = opt.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_opt.Enabled)
        {
            _logger.LogInformation("ScanIngest is disabled.");
            return;
        }

        EnsureDirectories();
        _logger.LogInformation("ScanIngest started. Watching: {Incoming}", _opt.IncomingPath);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in ingest loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _opt.PollIntervalSeconds)), stoppingToken);
        }
    }

    private void EnsureDirectories()
    {
        Directory.CreateDirectory(_opt.IncomingPath);
        Directory.CreateDirectory(_opt.ProcessedPath);
        Directory.CreateDirectory(_opt.UnmatchedPath);
        Directory.CreateDirectory(_opt.ErrorPath);
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        if (!Directory.Exists(_opt.IncomingPath)) return;

        var files = Directory.EnumerateFiles(_opt.IncomingPath)
            .Where(f => _opt.AllowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
            .ToList();

        foreach (var path in files)
        {
            if (ct.IsCancellationRequested) return;
            await ProcessSingleAsync(path, ct);
        }
    }

    private async Task ProcessSingleAsync(string path, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var fileName = Path.GetFileName(path);

        try
        {
            if (!await WaitUntilReadyAsync(path, ct))
            {
                _logger.LogWarning("File not ready: {File}", fileName);
                return;
            }

            if (!_parser.TryParse(fileName, out var info, out var parseError) || info is null)
            {
                _logger.LogWarning("Unmatched filename: {File} - {Err}", fileName, parseError);
                MoveSafe(path, Path.Combine(_opt.UnmatchedPath, fileName));
                return;
            }

            var ext = info.Extension;
            var size = new FileInfo(path).Length;
            if (size <= 0 || size > _opt.MaxFileSizeMB * 1024L * 1024L)
            {
                _logger.LogWarning("Invalid size: {File}, {Size} bytes", fileName, size);
                MoveSafe(path, Path.Combine(_opt.UnmatchedPath, fileName));
                return;
            }

            // DB scope
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // پیدا کردن کارمند
            var emp = await db.Employees
                .FirstOrDefaultAsync(e => e.NationalCode == info.NationalCode, ct);

            if (emp is null)
            {
                _logger.LogWarning("Employee not found (NC: {NC}) for {File}", info.NationalCode, fileName);
                MoveSafe(path, Path.Combine(_opt.UnmatchedPath, fileName));
                return;
            }

            // خواندن محتوا
            byte[] data;
            await using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using var ms = new MemoryStream();
                await fs.CopyToAsync(ms, ct);
                data = ms.ToArray();
            }

            var contentType = MimeHelper.FromExtension(info.Extension);

            var doc = new Document
            {
                EmployeeId = emp.Id,
                Title = null, // این الگو عنوان ندارد؛ در صورت نیاز بعداً تولید می‌کنیم
                Category = info.Category,
                FileName = fileName,
                ContentType = contentType,
                Size = data.LongLength,
                Data = data,
                UploadedOn = DateTimeOffset.UtcNow
            };

            db.Documents.Add(doc);
            await db.SaveChangesAsync(ct);

            // انتقال به Processed با جلوگیری از برخورد
            var targetName = EnsureUniqueInProcessed(fileName, info, ext);
            MoveSafe(path, Path.Combine(_opt.ProcessedPath, targetName));

            _logger.LogInformation("Ingest OK: {File} -> DocId={DocId} in {Ms}ms", targetName, doc.Id, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting file: {File}", fileName);
            MoveSafe(path, Path.Combine(_opt.ErrorPath, fileName));
        }
        finally
        {
            sw.Stop();
        }
    }

    private string EnsureUniqueInProcessed(string originalName, ScanFileNameInfo info, string ext)
    {
        var target = Path.Combine(_opt.ProcessedPath, originalName);
        if (!File.Exists(target)) return originalName;

        // اگر اسم برخورد کرد، شمارنده NNN را به بعدی افزایش بده
        var nextSeq = FileNameGenerator.GetNextSequence(_opt.ProcessedPath, info.NationalCode, info.Category, info.DateUtc, ext);
        var newName = FileNameGenerator.Generate(info.NationalCode, info.Category, info.DateUtc, nextSeq, ext);
        return newName;
    }

    private async Task<bool> WaitUntilReadyAsync(string path, CancellationToken ct)
    {
        // فایل باید برای چند ثانیه بدون تغییر بماند و قابل باز شدن باشد
        var lastSize = -1L;
        for (int i = 0; i < Math.Max(1, _opt.FileReadyDelaySeconds); i++)
        {
            if (ct.IsCancellationRequested) return false;
            try
            {
                var fi = new FileInfo(path);
                if (!fi.Exists) return false;

                if (fi.Length > 0 && fi.Length == lastSize)
                {
                    // یک بار بدون تغییر ماند؛ سعی می‌کنیم باز کنیم
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return true;
                }
                lastSize = fi.Length;
            }
            catch
            {
                // در حال نوشتن است؛ کمی صبر می‌کنیم
            }
            await Task.Delay(1000, ct);
        }
        return false;
    }

    private static void MoveSafe(string src, string dest)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        if (File.Exists(dest))
        {
            var name = Path.GetFileNameWithoutExtension(dest);
            var ext = Path.GetExtension(dest);
            var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            dest = Path.Combine(Path.GetDirectoryName(dest)!, $"{name}__dup_{ts}{ext}");
        }
        File.Move(src, dest, overwrite: false);
    }
}
