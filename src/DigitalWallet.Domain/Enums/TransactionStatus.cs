namespace DigitalWallet.Domain.Enums;
/// <summary>
/// Lifecycle state of a transaction
/// </summary>
public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Reversed = 4
}
