namespace DigitalWallet.Application.Features.Accounts.Commands;
/// <summary>
/// Command to create a new wallet account for the current user
/// </summary>
[Idempotent]
public class CreateAccountCommand : IRequest<AccountDto>
{
    public string IdempotencyKey { get; set; } = string.Empty; // for idempotency attribute
    public string CurrencyCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Personal";
    public string Name { get; set; } = string.Empty;
}