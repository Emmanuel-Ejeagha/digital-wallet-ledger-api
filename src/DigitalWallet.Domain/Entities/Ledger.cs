using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities
{
    /// <summary>
    /// Main ledger that contains all financial transactions
    /// CRITICAL: Central source of truth for all money movements
    /// </summary>
    public sealed class Ledger : AuditableEntity
    {
        public string Name { get; private set; }
        public string TenantId { get; private set; }
        public bool IsActive { get; private set; }

        // All ledger entries belong to this ledger
        public IReadOnlyCollection<LedgerEntry> Entries => _entries.AsReadOnly();
        private readonly List<LedgerEntry> _entries = new();

        private Ledger() { }

        public static Ledger Create(string name, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Ledger name is required");
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new DomainException("Tenant ID is required");

            return new Ledger
            {
                Name = name.Trim(),
                TenantId = tenantId,
                IsActive = true
            };
        }

        // Core financial operation: Record a transaction with double-entry
        public FinancialTransaction RecordTransaction(
            string description,
            TransactionType type,
            List<LedgerEntry> entries,
            TransactionReference? reference = null)
        {
            if (!IsActive)
                throw new InvalidTransactionException("Ledger is not active");

            if (entries.Count < 2)
                throw new InvalidTransactionException("Transaction must have at least 2 entries");

            // Validate double-entry accounting: Debits must equal Credits
            ValidateDoubleEntry(entries);

            // Create the transaction
            var transaction = FinancialTransaction.Create(
                description,
                type,
                reference ?? TransactionReference.GenerateSystemReference(),
                TenantId);

            // Link entries to transaction
            foreach (var entry in entries)
            {
                entry.LinkToTransaction(transaction.Id);
                _entries.Add(entry);
            }

            transaction.AddLedgerEntries(entries);

            return transaction;
        }

        private void ValidateDoubleEntry(List<LedgerEntry> entries)
        {
            decimal totalDebits = 0;
            decimal totalCredits = 0;

            foreach (var entry in entries)
            {
                if (entry.EntryType == EntryType.Debit)
                    totalDebits += entry.Amount.Amount;
                else if (entry.EntryType == EntryType.Credit)
                    totalCredits += entry.Amount.Amount;
            }

            if (totalDebits != totalCredits)
            {
                throw new InvalidTransactionException(
                    $"Double-entry validation failed: Debits ({totalDebits}) ≠ Credits ({totalCredits})");
            }
        }

        // Get all transactions for a specific account
        public List<FinancialTransaction> GetAccountTransactions(Guid accountId)
        {
            return _entries
                .Where(e => e.AccountId == accountId)
                .Select(e => e.Transaction)
                .Distinct()
                .ToList();
        }
    }
}