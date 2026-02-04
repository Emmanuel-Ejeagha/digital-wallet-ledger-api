using System;

namespace DigitalWallet.Domain.Enums;
/// <summary>
/// Chart of accounts classification
/// Determines whether account balance is normally DEBIT or CREDIT
/// </summary>
public enum AccountType
{
    // Asset accounts (Normal balance: DEBIT)
    Asset = 1,          // Cash, bank accounts, receivables
    WalletAsset = 2,    // Customer wallet balance (Asset for customer) 

    //  Liability accounts (Normal balance: CREDIT)
    Liability = 10,         // Payables, debts
    WalletLiability = 11,   // Customer wallet balance (Liablity for company)

    // Equity accounts (Normal balance: CREDIT)
    Equity = 20,            // Owner's equity

    // Income accounts (Normal balance: CREDIT)
    Revenue = 30,           // Sales, fees earned

    //  Expense accounts (Normal balance: DEBIT)
    Expense = 40
}
