using DigitalWallet.Domain.Base;
using DigitalWallet.Domain.Exceptions;

namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Money value object. Encapsulates amount and currency. Immutable. 
/// Amount is stored as decimal with full precision. Non-negative enforced.
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    /// <summary>
    /// Creates a Money instance. Validates that the amount does not exceed the currency's decimal places.
    /// </summary>
    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative");
        if (currency == null)
            throw new ArgumentNullException(nameof(currency));

        // Validate that the amount has no more decimal places than the currency allows
        var scale = BitConverter.GetBytes(decimal.GetBits(amount)[3])[2]; // number of decimal places

        if (scale > currency.DecimalPlaces)
            throw new DomainException($"Amount {amount} has more decimal places ({scale}) than allowed for currency {currency.Code} ({currency.DecimalPlaces}).");

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a Money instance from the smallest unit of the currency (e.g., cents).
    /// </summary>
    /// <param name="amountInSmallestUnit">e.g., 100 for $1.00</param>
    public static Money FromSmallestUnit(long amountInSmallestUnit, Currency currency)
    {
        if (currency == null) throw new ArgumentNullException(nameof(currency));
        decimal divisor = (decimal)Math.Pow(10, currency.DecimalPlaces);
        decimal amount = amountInSmallestUnit / divisor;
        return new Money(amount, currency);
    }

    /// <summary>
    /// Rounds the amount to the currency's decimal places using the specified rounding mode.
    /// Default is MidpointRounding.ToEven (banker's rounding).
    /// </summary>
    public Money Round(MidpointRounding roundingMode = MidpointRounding.ToEven)
    {
        var rounded = Math.Round(Amount, Currency.DecimalPlaces, roundingMode);
        return new Money(rounded, Currency);
    }

    // Operator overloads for convenience (only same currency allowed)
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("CAnnot add Money with different currencies.");
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot substract money with different currencies");

        if (left.Amount < right.Amount)
            throw new DomainException("insufficient funds for substraction.");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency.Symbol}{Amount.ToString("F{Currency.DecimalPlaces}")}";
    
}
