using System;
using DigitalWallet.Domain.Events;
using MediatR;

namespace DigitalWallet.Application.Features.Transaction.Notifications;
/// <summary>
/// MediatR notification that wraps the domain event
/// </summary>
public class MoneyTransferredNotification : INotification
{
    public MoneyTransferredEvent DomainEvent { get; }

    public MoneyTransferredNotification(MoneyTransferredEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
