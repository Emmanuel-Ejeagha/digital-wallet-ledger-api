namespace DigitalWallet.Domain.Enums;
/// <summary>
/// Wallet lifecycle status
/// CRITICAl: Only Active wallets can perform transactons
/// </summary>
public enum WalletStatus
{
    Pending = 1,    // Created but not yet activated
    Active = 2,     // Can perform transactions
    Suspended = 3,  // Temporarily blocked (fraud check)
    Frozen = 4,     // Legally blocked (court order)
    Closed = 5      // Permanently terminated
}
