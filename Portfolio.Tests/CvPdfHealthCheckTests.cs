using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Portfolio.Observability;
using Xunit;

namespace Portfolio.Tests;

public class CvPdfHealthCheckTests
{
    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public async Task MissingFile_ReturnsDegraded()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        var env = new FakeWebHostEnvironment { WebRootPath = root };
        var check = new CvPdfHealthCheck(env);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Degraded, result.Status);
    }

    [Fact]
    public async Task ExistingFile_ReturnsHealthy()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var filesDir = Path.Combine(root, "files");
        Directory.CreateDirectory(filesDir);
        await File.WriteAllTextAsync(Path.Combine(filesDir, "TomaszKraftCV.pdf"), "dummy");

        var env = new FakeWebHostEnvironment { WebRootPath = root };
        var check = new CvPdfHealthCheck(env);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }
}
