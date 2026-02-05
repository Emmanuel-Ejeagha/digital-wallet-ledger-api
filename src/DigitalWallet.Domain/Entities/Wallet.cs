using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Core entity representing a user's digital wallet
/// CRITICAL: Wallet balance is derived from ledger entries, not stored directlyg
/// </summary>
public sealed class Wallet : AuditableEntity
{
    // User identifier from Auth0
    public string UserId { get; private set; }

    // Tenant identifier for multi-tenant isolation
    public string TenantId { get; private set; }

    // Wallet metadata
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public WalletStatus Status { get; private set; }
    public Currency Currency { get; private set; }

    // Ledger account for this wallet (double-entry accounting)
    public Guid AccountId { get; private set; }

    // Related entities (EF Core will handle these as navigation proporties)
    public Account Account { get; private set; } = null!;
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();
    private readonly List<LedgerEntry> _ledgerEntries = new();

    // Private constructor for EF Core
    private Wallet() { }

    // Factory method for creating new wallets
    public static Wallet Create(
        string userId,
        string tenantId,
        string name,
        Currency currency,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required");

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new DomainException("Tenant ID is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Wallet name is required");

        var wallet = new Wallet
        {
            UserId = userId,
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description?.Trim(),
            Status = WalletStatus.Pending, // Must be activated before use
            Currency = currency
        };

        // Create initial ledger account for this wallet
        wallet.Account = Account.Create(
            $"Wallet Account: {name}",
            AccountType.WalletAsset,
            currency,
            wallet.Id);

        wallet.AccountId = wallet.Account.Id;

        wallet.AddDomainEvent(new WalletCreatedEvent(wallet.Id, userId, tenantId));

        return wallet;
    }

    // Business operations
    public void Activate()
    {
        if (Status != WalletStatus.Pending)
            throw new InvalidTransactionException($"Wallet is not in Pending state. Current: {Status}");

        Status = WalletStatus.Active;
        AddDomainEvent(new WalletActivatedEvent(Id));
    }

    public void Suspend(string reason)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidTransactionException($"Only Active waqllets can be suspended. Current: {Status}");

        Status = WalletStatus.Suspended;
        AddDomainEvent(new WalletSuspendedEvent(Id, reason));
    }

    public void Close(string reason)
    {
        if (Status == WalletStatus.Closed)
            throw new InvalidTransactionException("Wallet is already closed");

        // Check if wallet has zero balance before closing
        var balance = CalculateBalance();
        if (!balance.IsZero())
            throw new InvalidTransactionException($"Cannot close wallet with no-zero balance: {balance}");

        Status = WalletStatus.Closed;
        AddDomainEvent(new WalletClosedEvent(Id, reason));
    }

    // Financial operations (balance calculation)
    public Money CalculateBalance()
    {
        // Sum all ledger entries for this wallet's account
        var total = _ledgerEntries
            .Where(e => e.IsValidForBalance())
            .Sum(e => e.GetSignedAmount());

        return Money.Create(total, Currency);
    }

    public Money CalculateAvailableBalance()
    {
        var balance = CalculateBalance();

        return balance;
    }

    // Validation methods
    public bool CanAcceptTransactions()
    {
        return Status == WalletStatus.Active;
    }

    public bool HasSufficientBalance(Money amount)
    {
        if (!IsSameCurrency(amount))
            return false;

        return CalculateBalance() >= amount;
    }

    private bool IsSameCurrency(Money money)
    {
        return Currency == money.Currency;
    }

    // Doamin events
    public record WalletCreatedEvent(
        Guid WalletId,
        string UserId,
        string TenantId) : IDomainEvent;

    public record WalletActivatedEvent(Guid WalletId) : IDomainEvent;

    public record WalletSuspendedEvent(
        Guid WalletId,
        string Reason) : IDomainEvent;

    public record WalletClosedEvent(
        Guid WalletId,
        string Reason) : IDomainEvent;
}
