using System;
using DigitalWallet.Domain.Base;
using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Domain.Events;

public class MoneyTransferredEvent : IDomainEvent
{
    public Transaction Transaction { get; }
    
    public MoneyTransferredEvent(Transaction transaction)
    {
        Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }
}
