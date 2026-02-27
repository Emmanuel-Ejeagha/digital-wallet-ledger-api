namespace DigitalWallet.Application.DTOs;
/// <summary>
/// Data transfer object for Transaction aggregate.
/// </summary>
public class TransactionDto : IMapFrom<Transaction>
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public List<LedgerEntryDto> Entries { get; set; } = new();
}

public class LedgerEntryDto : IMapFrom<LedgerEntry>
{
    public Guid AccountId { get; set; }
    public string Type { get; set; } = string.Empty;
    public MoneyDto Amount { get; set; } = new();
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}


