using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Ledger Account for double-entry accounting
/// CRITICAL: Every financial transaction affects at least two accounts
/// </summary>
public sealed class Account : AuditabilityEntity
{
    // Account identification
    public string AccountNumber { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Financial classification
    public AccountType Type { get; private set; }
    public Currency Currency { get; private set; }

    // Relationships
    public Guid? WalletId { get; private set; }
    public Wallet? Wallet { get; private set; }

    // Ledger entries for this account
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

    private readonly List<LedgerEntry> _ledgerEntries = new();

    // Computed properties
    public Money Balance => CalculateBalance();
    public bool IsWalletAccount => WalletId.HasValue;

    private Account() { }

    public static Account Create(
        string name,
        AccountType type,
        Currency currency,
        Guid? walletId = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Account name is required");

        var account = new Account
        {
            AccountNumber = GenerateAccountNumber(type),
            Name = name.Trim(),
            Description = description?.Trim(),
            Type = type,
            Currency = currency,
            WalletId = walletId
        };

        return account;
    }

    private static string GenerateAccountNumber(AccountType type)
    {
        var prefix = type switch
        {
            AccountType.Asset => "1000",
            AccountType.WalletAsset => "1100",
            AccountType.Liability => "2000",
            AccountType.WalletLiability => "2100",
            AccountType.Equity => "3000",
            AccountType.Revenue => "4000",
            AccountType.Expense => "5000",
            _ => "0000"
        };

        return $"{prefix}-{Guid.NewGuid():N}";
    }

    // Calculate current balance based on ledger entries
    public Money CalculateBalance()
    {
        var total = _ledgerEntries
            .Where(e => e.IsValidForBalance())
            .Sum(e => e.GetSignedAmount());

        return Money.Create(total, Currency);
    }

    // Determine normal balance for this account type
    public bool IsNormalBalanceDebit()
    {
        return Type switch
        {
            AccountType.Asset => true,
            AccountType.WalletAsset => true,
            AccountType.Expense => true,
            _ => false
        };
    }

    // Validate if amount is valid for this account type
    public void ValidateAmount(EntryType entryType, Money amount)
    {
        if (Currency != amount.Currency)
            throw new InvalidTransactionException(
                $"Currency mismatch: Account uses {Currency.Code}, amount is {amount.Currency.Code}");

        // For normal debit accounts, debits increase, credits decrease
        // For normal credit accounts, credits increase, debits decrease
        var isDebit = entryType == EntryType.Debit;
        var isNormalDebitAccount = IsNormalBalanceDebit();

        // Check for unsual situations (e.g., negative balance for asset accounts)
        var newBalance = isDebit == isNormalDebitAccount
            ? Balance.Add(amount)
            : Balance.Subtract(amount);

        // Some accounts (like equity) shouldn't go negative
        if (Type == AccountType.Equity && newBalance.IsNegative())
            throw new InvalidTransactionException($"Account {Name} cannot have negative equity");
    }
}
