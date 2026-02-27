namespace DigitalWallet.Application.Features.Transaction.Queries;
/// <summary>
/// Query to retrieve paginated transaction history for an account,
/// optionally filtered by date range.
/// </summary>
public class GetTransactionHistoryQuery : IRequest<List<TransactionDto>>
{
    public Guid AccountId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}