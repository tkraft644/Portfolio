using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Portfolio.Observability;

public static class PortfolioTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Portfolio");
    public static readonly Meter Meter = new("Portfolio");

    public static readonly Counter<long> CvSendAttempts =
        Meter.CreateCounter<long>("portfolio.cv_send.attempts");

    public static readonly Counter<long> CvSendSuccesses =
        Meter.CreateCounter<long>("portfolio.cv_send.successes");

    public static readonly Counter<long> CvSendFailures =
        Meter.CreateCounter<long>("portfolio.cv_send.failures");

    public static readonly Histogram<double> CvSendDurationMs =
        Meter.CreateHistogram<double>("portfolio.cv_send.duration_ms", unit: "ms");
}
