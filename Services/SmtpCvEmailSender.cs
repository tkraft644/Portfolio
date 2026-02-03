using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Portfolio.Models;

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
        if (!_emailSettings.Enabled)
        {
            throw new InvalidOperationException("CV sending is disabled.");
        }

        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new ArgumentException("Recipient email is required.", nameof(recipientEmail));
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

        await client.SendMailAsync(message);
    }
}
