namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Registry of all supported currencies. Useful for validation and enumeration.
/// </summary>
public static class CurrencyRegistry
{
    private static readonly HashSet<Currency> _supported = new()
    {
        Currency.NGN,
        Currency.USD,
        Currency.EUR,
        Currency.GBP
    };

    public static IReadOnlyCollection<Currency> Supported => _supported.ToList().AsReadOnly();

    public static bool IsSupported(Currency currency) => _supported.Contains(currency);

    public static Currency FromCode(string code)
    {
        return _supported.FirstOrDefault(c => c.Code == code)
            ?? throw new DomainException($"Currency code '{code}' is not supported");
    }
}
