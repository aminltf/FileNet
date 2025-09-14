using FileNet.WebFramework.ScanIngest;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace FileNet.Tests;

public class ScanIngestHostedServiceTests
{
    [Fact]
    public async Task Disabled_Service_Exits_Without_Creating_Dirs()
    {
        var root = Path.Combine(Path.GetTempPath(), "ScanTest_" + Guid.NewGuid().ToString("N"));
        var opt = new ScanIngestOptions
        {
            Enabled = false,
            IncomingPath = Path.Combine(root, "incoming"),
            ProcessedPath = Path.Combine(root, "processed"),
            UnmatchedPath = Path.Combine(root, "unmatched"),
            ErrorPath = Path.Combine(root, "error"),
        };
        var svc = new ScanIngestHostedService(Options.Create(opt), new ScanFileNameParser(), NullLogger<ScanIngestHostedService>.Instance);

        await svc.StartAsync(CancellationToken.None);
        await svc.StopAsync(CancellationToken.None);

        Directory.Exists(opt.IncomingPath).Should().BeFalse();
        Directory.Exists(opt.ProcessedPath).Should().BeFalse();
    }

    [Fact]
    public async Task Enabled_Service_Creates_Dirs_And_Runs()
    {
        var root = Path.Combine(Path.GetTempPath(), "ScanTest_" + Guid.NewGuid().ToString("N"));
        try
        {
            var opt = new ScanIngestOptions
            {
                Enabled = true,
                IncomingPath = Path.Combine(root, "incoming"),
                ProcessedPath = Path.Combine(root, "processed"),
                UnmatchedPath = Path.Combine(root, "unmatched"),
                ErrorPath = Path.Combine(root, "error"),
                PollIntervalSeconds = 1
            };

            var svc = new ScanIngestHostedService(Options.Create(opt), new ScanFileNameParser(), NullLogger<ScanIngestHostedService>.Instance);

            using var cts = new CancellationTokenSource(millisecondsDelay: 1500);
            await svc.StartAsync(cts.Token);
            await svc.StopAsync(CancellationToken.None);

            Directory.Exists(opt.IncomingPath).Should().BeTrue();
            Directory.Exists(opt.ProcessedPath).Should().BeTrue();
            Directory.Exists(opt.UnmatchedPath).Should().BeTrue();
            Directory.Exists(opt.ErrorPath).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
