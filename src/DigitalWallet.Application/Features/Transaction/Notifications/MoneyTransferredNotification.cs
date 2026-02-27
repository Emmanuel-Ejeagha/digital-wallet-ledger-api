namespace DigitalWallet.Application.Features.Transaction.Notifications;
/// <summary>
/// MediatR notification that wraps the MoneyTransferredEvent domain event.
/// This allows the domain event to be published through MediatR without
/// coupling the domain to MediatR.
/// </summary>
public class MoneyTransferredNotification : INotification
{
    public MoneyTransferredEvent DomainEvent { get; }

    public MoneyTransferredNotification(MoneyTransferredEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
