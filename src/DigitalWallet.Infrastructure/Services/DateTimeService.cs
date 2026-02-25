namespace DigitalWallet.Infrastructure.Services;
/// <summary>
/// Provides current date and time.
/// </summary>
public class DateTimeService :IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}
