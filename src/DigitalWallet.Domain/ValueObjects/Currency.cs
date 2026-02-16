using DigitalWallet.Domain.Base;

namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Currency value object. Immutable, with predefined static instances for common currencies.
/// </summary>
public sealed class Currency : ValueObject
{
    public string Code { get; } = string.Empty;
    public int DecimalPlaces { get; }
    public string Symbol { get; } = string.Empty;
    public string Name { get; } = string.Empty;

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
