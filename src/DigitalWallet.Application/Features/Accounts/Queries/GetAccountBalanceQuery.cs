namespace DigitalWallet.Application.Features.Accounts.Queries;
/// <summary>
/// Query to get the balance of a specific
/// </summary>
public class GetAccountBalanceQuery : IRequest<AccountDto>
{
    public Guid AccountId { get; set; }
}