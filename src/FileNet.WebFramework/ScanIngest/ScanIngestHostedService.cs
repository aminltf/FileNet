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
    private HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase);

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
        _allowed = _opt.AllowedExtensions?.Select(e => e.ToLowerInvariant()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                   ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_opt.Enabled)
        {
            _logger.LogInformation("ScanIngest is disabled.");
            return;
        }

        EnsureDirectories();
        _logger.LogInformation("ScanIngest started. Incoming: {incoming}", _opt.IncomingPath);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in scan loop.");
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

    private async Task ProcessOnceAsync(CancellationToken ct)
    {
        var files = Directory.EnumerateFiles(_opt.IncomingPath)
            .Where(p => _allowed.Contains(Path.GetExtension(p).ToLowerInvariant()))
            .ToArray();

        if (files.Length == 0) return;

        foreach (var path in files)
        {
            if (ct.IsCancellationRequested) break;
            await ProcessFileSafeAsync(path, ct);
        }
    }

    private async Task ProcessFileSafeAsync(string path, CancellationToken ct)
    {
        try
        {
            if (!await IsFileReadyAsync(path, ct))
            {
                _logger.LogDebug("File not ready yet: {path}", path);
                return;
            }

            await ProcessFileAsync(path, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Processing failed for {path}", path);
            await MoveWithReasonAsync(path, _opt.ErrorPath, "Unhandled error: " + ex.Message, ct);
        }
    }

    private async Task<bool> IsFileReadyAsync(string path, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(Math.Max(0, _opt.FileReadyDelaySeconds)), ct);

        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return fs.Length > 0;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private async Task ProcessFileAsync(string path, CancellationToken ct)
    {
        var fileName = Path.GetFileName(path);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (!_parser.TryParse(fileName, out var meta, out var error))
        {
            await MoveWithReasonAsync(path, _opt.UnmatchedPath, $"Pattern mismatch: {error}", ct);
            return;
        }

        var fi = new FileInfo(path);
        if (fi.Length > (long)_opt.MaxFileSizeMB * 1024 * 1024)
        {
            await MoveWithReasonAsync(path, _opt.ErrorPath, $"File too large ({fi.Length} bytes).", ct);
            return;
        }
        if (!_allowed.Contains(ext))
        {
            await MoveWithReasonAsync(path, _opt.UnmatchedPath, $"Extension {ext} not allowed.", ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var employeeId = await db.Employees
            .Where(e => e.NationalCode == meta!.NationalCode)
            .Select(e => (Guid?)e.Id)
            .FirstOrDefaultAsync(ct);

        if (employeeId is null)
        {
            await MoveWithReasonAsync(path, _opt.UnmatchedPath, $"Employee with NC {meta.NationalCode} not found.", ct);
            return;
        }

        var bytes = await File.ReadAllBytesAsync(path, ct);
        var contentType = MimeHelper.FromExtension(ext);

        var doc = new Document
        {
            EmployeeId = employeeId.Value,
            Title = meta.Title,
            Category = meta.Category,
            FileName = fileName,
            ContentType = contentType,
            Size = bytes.LongLength,
            Data = bytes,
            UploadedOn = meta.TimestampUtc
        };

        db.Documents.Add(doc);
        await db.SaveChangesAsync(ct);

        var newBase = Path.GetFileNameWithoutExtension(fileName) + $"__DOC_{doc.Id:N}";
        var dest = Path.Combine(_opt.ProcessedPath, newBase + ext);
        await MoveFileAsync(path, dest, ct);

        _logger.LogInformation("Ingested file for Employee {EmployeeId} as Document {DocumentId}", employeeId, doc.Id);
    }

    private async Task MoveWithReasonAsync(string src, string destDir, string reason, CancellationToken ct)
    {
        var baseName = Path.GetFileName(src);
        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var destPath = Path.Combine(destDir, $"{ts}__{baseName}");
        await MoveFileAsync(src, destPath, ct);

        try
        {
            await File.WriteAllTextAsync(destPath + ".txt", reason, ct);
        }
        catch { /* best effort */ }

        _logger.LogWarning("Moved {src} -> {destDir}. Reason: {reason}", baseName, destDir, reason);
    }

    private static async Task MoveFileAsync(string src, string dst, CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
        if (File.Exists(dst))
        {
            var dir = Path.GetDirectoryName(dst)!;
            var name = Path.GetFileNameWithoutExtension(dst);
            var ext = Path.GetExtension(dst);
            dst = Path.Combine(dir, $"{name}_{Guid.NewGuid():N}{ext}");
        }

        using (var srcStream = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var dstStream = new FileStream(dst, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await srcStream.CopyToAsync(dstStream, ct);
        }
        File.Delete(src);
    }
}
