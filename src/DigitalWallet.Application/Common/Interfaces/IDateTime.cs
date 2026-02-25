namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Provides the current date and time. Useful for unit testing to control time.
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
