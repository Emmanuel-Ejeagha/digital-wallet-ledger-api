namespace DigitalWallet.Application.Features.Transaction.Commands;

[Idempotent]
public class TransferCommand : IRequest<TransactionDto>
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
