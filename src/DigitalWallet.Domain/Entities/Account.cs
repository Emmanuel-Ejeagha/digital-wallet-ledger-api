namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Account aggregate root. Stores current balance (derived from ledger entries, with concurrency control)
/// </summary>
public class Account : AggregateRoot
{
    public Guid UserId { get; private set; }
    public AccountType Type { get; private set; }
    public Currency Currency { get; private set; }
    public decimal Balance { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public byte[] ConcurrencyToken { get; private set; }

    private string _currencyCodeDummy;

    [NotMapped]
    public string CurrencyCode
    {
        get => Currency.Code;
        private set => _currencyCodeDummy = value;
    }

    private Account() { } // EF Core

    public Account(Guid userId, AccountType type, Currency currency, string name) : base()
    {
        UserId = userId;
        Type = type;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Balance = 0;
        IsActive = true;

        AddDomainEvent(new AccountCreatedEvent(this));
    }

    /// <summary>
    /// Apllies a debit (increase) to the account balance.
    /// </summary>
    public void ApplyDebit(Money amount)
    {
        if (amount.Currency != Currency)
            throw new DomainException($"Cannot debit account with currency {Currency.Code} using {amount.Currency.Code}");
        if (!IsActive)
            throw new DomainException("Account is not active.");

        Balance += amount.Amount;
    }

    /// <summary>
    /// Applies a credit (decrease) to the account balance.
    /// </summary>
    public void ApplyCredit(Money amount)
    {
        if (amount.Currency != Currency)
            throw new DomainException($"Cannot credit account with currency {Currency.Code} using {amount.Currency.Code}");
        if (!IsActive)
            throw new DomainException("Account is not active");
        if (Balance < amount.Amount)
            throw new DomainException("Insufficient balance for credit.");

        Balance -= amount.Amount;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
