using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileNet.WebFramework.ScanIngest;

public sealed class ScanIngestHostedService : BackgroundService
{
    private readonly ILogger<ScanIngestHostedService> _logger;
    private readonly ScanIngestOptions _opt;
    private readonly ScanFileNameParser _parser;

    public ScanIngestHostedService(
        IOptions<ScanIngestOptions> opt,
        ScanFileNameParser parser,
        ILogger<ScanIngestHostedService> logger)
    {
        _logger = logger;
        _parser = parser;
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

        _logger.LogInformation("ScanIngest starting. Incoming: {Incoming}, Processed: {Processed}, Unmatched: {Unmatched}, Error: {Error}",
            _opt.IncomingPath, _opt.ProcessedPath, _opt.UnmatchedPath, _opt.ErrorPath);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("ScanIngest heartbeat...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScanIngest loop error.");
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
}
