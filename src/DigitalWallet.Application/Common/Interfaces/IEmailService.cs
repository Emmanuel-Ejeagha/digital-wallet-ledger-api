namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Service for sending notification to users. Implemented in Inrastucture.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email an email asynchronously..
    /// </summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">HTML or plain text body</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
