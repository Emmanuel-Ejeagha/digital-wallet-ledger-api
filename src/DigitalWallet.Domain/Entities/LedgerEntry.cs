using DigitalWallet.Domain.Base;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Represents one side of a double-entry transaction. Once created, it cannot be modified
/// </summary>
public class LedgerEntry : Entity
{
    public Guid AccountId { get; private set; }
    public Guid TransactionId { get; private set; }
    public EntryType Type { get; private set; }
    public Money Amount { get; private set; }
    public decimal BalanceAfter { get; set; } // Snapshot of account balance after entry (for audit)
    public DateTime CreatedAt { get; private set; }
    public string? Description { get; private set; }

    private LedgerEntry() { } // EF Core

    public LedgerEntry(Guid accountId, Guid transactionId, EntryType type, Money amount, decimal balanceAfter, string description) : base()
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        AccountId = accountId;
        TransactionId = transactionId;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        CreatedAt = DateTime.UtcNow;
        Description = description ?? string.Empty;
    }
}
