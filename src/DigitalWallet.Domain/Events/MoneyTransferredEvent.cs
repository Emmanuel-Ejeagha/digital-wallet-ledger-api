namespace DigitalWallet.Domain.Events;

public class MoneyTransferredEvent : IDomainEvent
{
    public Transaction Transaction { get; }
    
    public MoneyTransferredEvent(Transaction transaction)
    {
        Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }
}
