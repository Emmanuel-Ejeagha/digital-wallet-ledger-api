using System;

namespace DigitalWallet.Domain.Enums;
/// <summary>
/// Types of financial transactions in our sysytem
/// CRITICAL: This determines how we apply double entry rules
/// </summary>
public enum TransactionType
{
    // Wallet operations
    Deposit = 1,        // Adding money to wallet
    Withdrawal = 2,     // Removing money from wallet
    Transfer = 3,       // Moving money between wallets
    Adjustment = 4,     // Manual correction (admin only)

// System operations
    Fee = 10,           // Service fee deduction
    Interest = 11,      // Interest accural
    Refund = 12         // Transaction reversal
}
