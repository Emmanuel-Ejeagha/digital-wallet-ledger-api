using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Single entry in the double-entry accounting system
/// CRITICAL: Every financial transaction creates ≥ 2 ledger entries
/// </summary>
public class LedgerEntry : AuditableEntity
{
    // Parent transaction
    public Guid TransactionId { get; private set; }
    public FinancialTransaction Transaction { get; private set; } = null!;

    // Account affected by this entry
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;

    // Financial details
    public EntryType EntryType { get; private set; }
    public Money Amount { get; private set; } = null!;

    // Tracking
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ReversedAt { get; private set; }
    public Guid? ReveralOfEntryId { get; private set; }
    public LedgerEntry? ReversalOfEntry { get; private set; }

    // Description for this specific entry
    public string Description { get; private set; } = string.Empty;

    private LedgerEntry() { }

    public static LedgerEntry Create(
        Guid accountId,
        EntryType entryType,
        Money amount,
        DateTime effectiveDate,
        string description)
    {
        if (amount.IsZero())
            throw new InvalidTransactionException("Ledger entry amount cannot be zero");

        if (amount.IsNegative())
            throw new InvalidTransactionException("Ledger entry amount cannot be negative");

        return new LedgerEntry
        {
            AccountId = accountId,
            EntryType = entryType,
            Amount = amount,
            EffectiveDate = effectiveDate,
            Description = description?.Trim() ?? string.Empty
        };
    }

    // Link to parent transaction
    internal void LinkToTransaction(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    // Create a reversal entry (for reefunds, corrections)
    public LedgerEntry CreateReversal(string reason)
    {
        var reversalType = EntryType == EntryType.Debit
            ? EntryType.Credit
            : EntryType.Debit;

        var reversal = LedgerEntry.Create(
            AccountId,
            reversalType,
            Amount,
            DateTime.UtcNow,
            $"Reversal of {Description}: {reason}");

        reversal.ReveralOfEntryId = Id;

        return reversal;
    }

    // Get signed amount for balance calculations
    public decimal GetSignedAmount()
    {
        return EntryType == EntryType.Debit ? Amount.Amount : -Amount.Amount;
    }

    // Check if entry should be included in balance calculations
    public bool IsValidForBalance()
    {
        // Reversed entries don't affect current balance
        return ReversedAt == null;
    }
    
    // Reversed this entry
    public void Reverse(DateTime reversedAt)
    {
        if (ReversedAt.HasValue)
            throw new InvalidTransactionException("Entry is already reversed");

        ReversedAt = reversedAt;
    }
}
