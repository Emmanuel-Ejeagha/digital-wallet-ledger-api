namespace DigitalWallet.Domain.Events;

public class AccountCreatedEvent : IDomainEvent
{
    public Account Account { get; }

    public AccountCreatedEvent(Account account)
    {
        Account = account ?? throw new ArgumentNullException(nameof(account));
    }
}
