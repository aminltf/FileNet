using FileNet.WebFramework.ScanIngest;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace FileNet.Tests;

public class ScanIngestHostedServiceTests
{
    [Fact]
    public async Task Disabled_Service_Exits_Without_Creating_Dirs()
    {
        var root = Path.Combine(Path.GetTempPath(), "ScanTest_" + Guid.NewGuid().ToString("N"));
        try
        {
            var opt = new ScanIngestOptions
            {
                Enabled = false,
                IncomingPath = Path.Combine(root, "incoming"),
                ProcessedPath = Path.Combine(root, "processed"),
                UnmatchedPath = Path.Combine(root, "unmatched"),
                ErrorPath = Path.Combine(root, "error"),
            };

            var sp = new ServiceCollection().BuildServiceProvider();

            var svc = new ScanIngestHostedService(
                Options.Create(opt),
                new ScanFileNameParser(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                NullLogger<ScanIngestHostedService>.Instance);

            await svc.StartAsync(CancellationToken.None);
            await svc.StopAsync(CancellationToken.None);

            Directory.Exists(opt.IncomingPath).Should().BeFalse();
            Directory.Exists(opt.ProcessedPath).Should().BeFalse();
            Directory.Exists(opt.UnmatchedPath).Should().BeFalse();
            Directory.Exists(opt.ErrorPath).Should().BeFalse();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Enabled_Service_Creates_Dirs_And_Runs_Heartbeat()
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
                PollIntervalSeconds = 1,
                FileReadyDelaySeconds = 0
            };

            var sp = new ServiceCollection().BuildServiceProvider();

            var svc = new ScanIngestHostedService(
                Options.Create(opt),
                new ScanFileNameParser(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                NullLogger<ScanIngestHostedService>.Instance);

            await svc.StartAsync(CancellationToken.None);
            await Task.Delay(300);
            await svc.StopAsync(CancellationToken.None);

            Directory.Exists(opt.IncomingPath).Should().BeTrue();
            Directory.Exists(opt.ProcessedPath).Should().BeTrue();
            Directory.Exists(opt.UnmatchedPath).Should().BeTrue();
            Directory.Exists(opt.ErrorPath).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }
}
