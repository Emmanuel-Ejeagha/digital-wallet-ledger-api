using System;

namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Value objects representing a currency
/// IMMUTABLE: Once created, cannot be changed
/// CRITICAL: Financial systems must handle currencies correctly
/// </summary>
public class Currency : IEquatable<Currency>
{
    // ISO 427 currency code (USD, EURO, GBP, etc.)
    public string Code { get; }

    // Number of decimal places (NGN: 2, JPY: 0)
    public int DecimalPlaces { get; }

    // Currency symbol (₦, $, €, £,)
    public string Symbol { get; }

    // Common currencies as static properies
    public static Currency NGN => new("NGN", 2, "₦");
    public static Currency USD => new("USD", 2, "$");
    public static Currency EUR => new("EUR", 2, "€");
    public static Currency GBP => new("GBP", 2, "£");

    private Currency(string code, int decimalPlaces, string symbol)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            throw new DomainException("Currency code must be 3 letters (ISO 4217)");

        if (decimalPlaces < 0 || decimalPlaces > 8)
            throw new DomainException("Decimal places must be between 0 and 8");

        if (string.IsNullOrWhiteSpace(symbol))
            throw new DomainException("Decimal symbol is required");

        Code = code.ToUpperInvariant();
        DecimalPlaces = decimalPlaces;
        Symbol = symbol;
    }

    // Factory method for creating currencies
    public static Currency Create(string code, int decimalPlaces, string symbol)
    {
        return new Currency(code, decimalPlaces, symbol);
    }

    // Value objects must implement equality
    public bool Equals(Currency? other)
    {
        if (other is null) return false;
        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Currency);
    }


    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public static bool operator ==(Currency left, Currency right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Currency left, Currency right)
    {
        return !(left == right);
    }

    public override string ToString() => Code;
}
