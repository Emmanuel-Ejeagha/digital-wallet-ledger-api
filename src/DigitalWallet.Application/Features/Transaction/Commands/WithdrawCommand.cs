namespace DigitalWallet.Application.Features.Transaction.Commands;
/// <summary>
/// Command to withdraw money from a user's wallet (to system payout account).
/// </summary>
[Idempotent]
public class WithdrawCommand : IRequest<TransactionDto>
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = "Withdrawal";
}
