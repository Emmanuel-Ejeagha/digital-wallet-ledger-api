namespace DigitalWallet.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        private Money() { Currency = null!; } // EF Core

        public Money(decimal amount, Currency currency)
        {
            if (amount < 0)
                throw new DomainException("Money amount cannot be negative.");
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            // Validate that the amount has no more decimal places than the currency allows
            var scale = BitConverter.GetBytes(decimal.GetBits(amount)[3])[2]; // number of decimal places
            if (scale > currency.DecimalPlaces)
                throw new DomainException($"Amount {amount} has more decimal places ({scale}) than allowed for currency {currency.Code} ({currency.DecimalPlaces}).");

            Amount = amount;
            Currency = currency;
        }

        public static Money FromSmallestUnit(long amountInSmallestUnit, Currency currency)
        {
            if (currency == null) throw new ArgumentNullException(nameof(currency));
            decimal divisor = (decimal)Math.Pow(10, currency.DecimalPlaces);
            decimal amount = amountInSmallestUnit / divisor;
            return new Money(amount, currency);
        }

        public Money Round(MidpointRounding roundingMode = MidpointRounding.ToEven)
        {
            var rounded = Math.Round(Amount, Currency.DecimalPlaces, roundingMode);
            return new Money(rounded, Currency);
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new DomainException("Cannot add money with different currencies.");
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new DomainException("Cannot subtract money with different currencies.");
            if (left.Amount < right.Amount)
                throw new DomainException("Insufficient funds for subtraction.");
            return new Money(left.Amount - right.Amount, left.Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Currency.Symbol}{Amount.ToString($"F{Currency.DecimalPlaces}")}";
    }
}