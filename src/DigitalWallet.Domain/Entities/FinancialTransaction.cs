using System;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// A complete financial transaction with double-entry accounting
/// CRITICAL: Atomic unit of work that maintains financial integrity
/// </summary>
public sealed class FinancialTransaction : AuditableEntity
{
    // Transaction metadata
    public string Description { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionReference Reference { get; private set; }
    public string TenantId { get; private set; }

    // Status tracking
    public bool IsCompleted { get; private set; }
    public bool IsReversed { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? ReversedAt { get; private set; }
    public string? ReversalReason { get; private set; }

    // All ledger entries for this transaction
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();
    private readonly List<LedgerEntry> _ledgerEntries = new();

    private FinancialTransaction() { }

    public static FinancialTransaction Create(
        string description,
        TransactionType type,
        TransactionReference reference,
        string tenantId)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description is required");

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new DomainException("Tenant ID is required");

        return new FinancialTransaction
        {
            Description = description.Trim(),
            Type = type,
            Reference = reference,
            TenantId = tenantId,
            IsCompleted = false,
            IsReversed = false
        };
    }

    // Add ledger entries to transactions
    internal void AddLedgerEntries(List<LedgerEntry> entries)
    {
        if (IsCompleted)
            throw new InvalidTransactionException("Cannot add entries to completed transaction");

        _ledgerEntries.AddRange(entries);
    }

    // Mark transaction as completed
    public void Complete()
    {
        if (IsCompleted)
            throw new InvalidTransactionException("Transaction is already completed");

        if (_ledgerEntries.Count < 2)
            throw new InvalidTransactionException("Transaction must have at least 2 entries");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionCompletedEvent(Id, Type, Reference));
    }

    // Reverse entire transaction
    public void Reverse(string reason)
    {
        if (!IsCompleted)
            throw new InvalidTransactionException("Only completed transactions can be reversed");

        if (IsReversed)
            throw new InvalidTransactionException("Transaction is already reversed");

        // Create reversal entries for all original entries
        // foreach (var entry in _ledgerEntries)
        // {
        //     if (entry.IsValidForBalance())
        //     {
        //         var reversal = entry.CreateReversal(reason);
        //         _ledgerEntries.Add(reversal);
        //     }
        // }
        var originals = _ledgerEntries
            .Where(e => e.IsValidForBalance())
            .ToList();

        foreach (var entry in originals)
        {
            _ledgerEntries.Add(entry.CreateReversal(reason));
        }


        IsReversed = true;
        ReversedAt = DateTime.UtcNow;
        ReversalReason = reason;

        AddDomainEvent(new TransactionReversedEvent(Id, reason));
    }

    // Get total amount of transaction
    public Money GetTotalAmount()
    {
        var debitTotal = _ledgerEntries
            .Where(e => e.EntryType == EntryType.Debit && e.IsValidForBalance())
            .Sum(e => e.Amount.Amount);

        // Assumming all amounts are same currency (validated during creation)
        var firstEntry = _ledgerEntries.FirstOrDefault();
        return firstEntry != null
            ? Money.Create(debitTotal, firstEntry.Amount.Currency)
            : Money.Zero(Currency.NGN);
    }

    // Get net effect for a specific account
    public Money GetNetEffectForAccount(Guid accountId)
    {
        var netAmount = _ledgerEntries
            .Where(e => e.AccountId == accountId && e.IsValidForBalance())
            .Sum(e => e.GetSignedAmount());

        var firstEntry = _ledgerEntries.FirstOrDefault();
        return firstEntry != null
            ? Money.Create(netAmount, firstEntry.Amount.Currency)
            : Money.Zero(Currency.NGN);
    }

    // Domain events
    public record TransactionCompletedEvent(
        Guid TransactionId,
        TransactionType Type,
        TransactionReference Reference) : IDomainEvent;

    public record TransactionReversedEvent(
        Guid TransactionId,
        string Reason) : IDomainEvent;
}
