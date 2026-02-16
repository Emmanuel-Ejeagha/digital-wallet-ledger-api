using DigitalWallet.Domain.Base;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Represents a financial transaction (e.g., transfer, deposit, withdrawal). Contains two or more ledger entries.
/// Enforces double-entry invariant: total debits = total credits.
/// </summary>
public class Transaction : AggregateRoot
{
    private readonly List<LedgerEntry> _entries = new();

    public string Reference { get; private set; } // Unique transaction reference (e.g., generated)
    public string Description { get; private set; }
    public IdempotencyKey IdempotencyKey { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public IReadOnlyCollection<LedgerEntry> Entries => _entries.AsReadOnly();

    private Transaction() { } // EF Core

    public Transaction(string reference, string description, IdempotencyKey idempotencyKey) : base()
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference cannot be empty.", nameof(reference));
        Reference = reference;
        Description = description ?? string.Empty;
        IdempotencyKey = idempotencyKey ?? throw new ArgumentNullException(nameof(idempotencyKey));
        Status = TransactionStatus.Pending;
    }

    /// <summary>
    /// Adds a ledger to the transaction. Must be called before completing the transaction.
    /// </summary>
    public void AddEntry(LedgerEntry entry)
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Cannot add entry to a non-pending transaction.");
        if (entry == null)
            throw new ArgumentNullException("Entry cannot be null");
        if (entry.TransactionId != Id)
            throw new DomainException("LedgerEntry.TransactionId must match Transaction.Id");
        _entries.Add(entry);
    }

    /// <summary>
    /// Completes the transaction, verfying that total debits equals total credits.
    /// </summary>
    public void Complete()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Transaction is not pending.");

        var totalDebits = _entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount.Amount);
        var totalCredits = _entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount.Amount);

        if (totalDebits != totalCredits)
            throw new DomainException($"Double-entry invariant violated: Debits ({totalDebits}) != Credits ({totalCredits}).");

        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new MoneyTransferredEvent(this));
    }

    /// <summary>
    /// Marks the transaction as failed (e.g., due to concurrency or business rule violation)
    /// </summary>
    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be marked failed.");
        Status = TransactionStatus.Failed;
    }

    /// <summary>
    /// Reverses a completed transaction by creating a new reversal transaction.
    /// </summary>
    public Transaction CreateReversal(string reversalReference, IdempotencyKey reversalIdempotencyKey)
    {
        if (Status != TransactionStatus.Completed)
            throw new DomainException("Only completed transactions can be reversed.");

        var reversal = new Transaction(reversalReference,
            $"Reversal of {Reference}",
            reversalIdempotencyKey);

        foreach (var entry in _entries)
        {
            var reversalType = entry.Type == EntryType.Debit
                ? EntryType.Credit
                : EntryType.Debit;
        }
        return reversal;
    }
}
