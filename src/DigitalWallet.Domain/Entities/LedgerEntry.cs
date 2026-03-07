namespace DigitalWallet.Domain.Entities
{
    public class LedgerEntry : Entity
    {
        public Guid AccountId { get; private set; }
        public Guid TransactionId { get; private set; }
        public EntryType Type { get; private set; }
        public Money Amount { get; private set; }
        public decimal BalanceAfter { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Description { get; private set; }

        private LedgerEntry()
        {
            Amount = null!;
            Description = null!;
        }

        public LedgerEntry(Guid accountId, Guid transactionId, EntryType type, Money amount, decimal balanceAfter, string description)
            : base()
        {
            AccountId = accountId;
            TransactionId = transactionId;
            Type = type;
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            BalanceAfter = balanceAfter;
            CreatedAt = DateTime.UtcNow;
            Description = description ?? string.Empty;
        }
    }
}