namespace DigitalWallet.Application.DTOs;
/// <summary>
/// Data transfer object for Account entity
/// </summary>
public class AccountDto : IMapFrom<Account>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public CurrencyDto Currency { get; set; } = new();
    public decimal Balance { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
