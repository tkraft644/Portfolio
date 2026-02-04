using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Portfolio.Observability;

public sealed class CvPdfHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _env;

    public CvPdfHealthCheck(IWebHostEnvironment env)
    {
        _env = env;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var cvPath = Path.Combine(_env.WebRootPath, "files", "TomaszKraftCV.pdf");

        return Task.FromResult(
            File.Exists(cvPath)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded("CV PDF is missing."));
    }
}
