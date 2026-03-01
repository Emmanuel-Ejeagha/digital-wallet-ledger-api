using DigitalWallet.Domain.Base;

namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Currency value object. Immutable, with predefined static instances for common currencies.
/// </summary>
public sealed class Currency : ValueObject
{
    public string Code { get; private set; } = string.Empty;
    public int DecimalPlaces { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    private Currency() { } // Ef Core

    // Public factory method for creating any currency (for testing or dynamic currencies)
    public static Currency Create(string code, int decimalPlaces, string symbol, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            throw new ArgumentException("Currency code must be a 3-letter ISO code.", nameof(code));
        if (decimalPlaces < 0 || decimalPlaces > 28)
            throw new ArgumentException("Decimal places must be between 0 and 28.", nameof(decimalPlaces));
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be empty.", nameof(symbol));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        return new Currency(code, decimalPlaces, symbol, name);
    }

    // Static properties for common currencies
    public static Currency NGN => new("NGN", 2, "₦", "Naira");
    public static Currency USD => new("USD", 2, "$", "US Dollar");
    public static Currency EUR => new("EUR", 2, "€", "Euro");
    public static Currency GBP => new("GBP", 2, "£", "Pounds");

    private Currency(string code, int decimalPlaces, string symbol, string name)
    {
        Code = code;
        DecimalPlaces = decimalPlaces;
        Symbol = symbol;
        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => $"{Code} ({Symbol})";    
}
