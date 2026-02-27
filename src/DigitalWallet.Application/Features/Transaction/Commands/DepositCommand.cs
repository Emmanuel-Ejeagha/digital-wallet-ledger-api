namespace DigitalWallet.Application.Features.Transaction.Commands;
/// <summary>
/// Command to deposit money into a user's wallet (from system reserve).
/// </summary>
[Idempotent]
public class DepositCommand : IRequest<TransactionDto>
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = "Deposit";
}
