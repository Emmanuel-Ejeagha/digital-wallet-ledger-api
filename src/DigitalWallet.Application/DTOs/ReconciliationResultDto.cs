namespace DigitalWallet.Application.DTOs;

public class ReconciliationResultDto
{
    public bool IsBalanced { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal Difference { get; set; }
    public List<string> Discripancies { get; set; } = new();
}