namespace DigitalWallet.Application.DTOs;

public class MoneyDto
{
    public decimal Amount { get; set; }
    public CurrencyDto Currency { get; set; } = new();
}
