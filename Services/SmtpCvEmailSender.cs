using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Portfolio.Models.Settings;
using Portfolio.Observability;

namespace Portfolio.Services;

public class SmtpCvEmailSender : ICvEmailSender
{
    private readonly IWebHostEnvironment _env;
    private readonly EmailSettings _emailSettings;

    public SmtpCvEmailSender(IWebHostEnvironment env, IOptions<EmailSettings> emailOptions)
    {
        _env = env;
        _emailSettings = emailOptions.Value;
    }

    public bool IsEnabled => _emailSettings.Enabled;

    public async Task SendCvAsync(string recipientEmail, CancellationToken cancellationToken = default)
    {
        PortfolioTelemetry.CvSendAttempts.Add(1);
        using var activity = PortfolioTelemetry.ActivitySource.StartActivity("cv.send");
        var sw = Stopwatch.StartNew();

        if (!_emailSettings.Enabled)
        {
            throw new InvalidOperationException("CV sending is disabled.");
        }

        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new ArgumentException("Recipient email is required.", nameof(recipientEmail));
        }

        var atIndex = recipientEmail.LastIndexOf('@');
        if (atIndex >= 0 && atIndex < recipientEmail.Length - 1)
        {
            activity?.SetTag("recipient.domain", recipientEmail[(atIndex + 1)..].Trim());
        }

        if (string.IsNullOrWhiteSpace(_emailSettings.Host) ||
            string.IsNullOrWhiteSpace(_emailSettings.User) ||
            string.IsNullOrWhiteSpace(_emailSettings.Password))
        {
            throw new InvalidOperationException(
                "EmailSettings is incomplete. Configure Host/User/Password using user-secrets or environment variables.");
        }

        var isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

        var subject = "CV – Tomasz Kraft";
        var body = isEnglish
            ? "Hi,\n\nPlease find attached my CV in PDF format.\n\nBest regards,\nTomasz Kraft"
            : "Cześć,\n\nW załączeniu przesyłam moje CV w formacie PDF.\n\nPozdrawiam,\nTomasz Kraft";

        using var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromDisplayName),
            Subject = subject,
            Body = body
        };

        message.To.Add(recipientEmail);

        var cvPath = Path.Combine(_env.WebRootPath, "files", "TomaszKraftCV.pdf");
        if (!File.Exists(cvPath))
        {
            throw new FileNotFoundException("CV file not found.", cvPath);
        }

        message.Attachments.Add(new Attachment(cvPath));

        cancellationToken.ThrowIfCancellationRequested();

        using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.User, _emailSettings.Password)
        };

        try
        {
            await client.SendMailAsync(message);
            PortfolioTelemetry.CvSendSuccesses.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch
        {
            PortfolioTelemetry.CvSendFailures.Add(1);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
        finally
        {
            sw.Stop();
            PortfolioTelemetry.CvSendDurationMs.Record(sw.Elapsed.TotalMilliseconds);
        }
    }
}
