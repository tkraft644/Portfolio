namespace Portfolio.Services;

public interface ICvEmailSender
{
    bool IsEnabled { get; }
    Task SendCvAsync(string recipientEmail, CancellationToken cancellationToken = default);
}
