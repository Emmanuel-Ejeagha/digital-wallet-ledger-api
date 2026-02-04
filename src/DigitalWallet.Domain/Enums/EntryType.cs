namespace DigitalWallet.Domain.Enums;
/// <summary>
///  Double-entry accounting: Every transaction has both DEBIT and CREDIT entries
/// DEBIT: Left side of accounting equation (Assest increase, Liabilites decrease)
/// CREDIT: Right side of accounting equation (Asses decrease, Liabilities increase)
/// </summary>
public enum EntryType
{
    Debit = 1, 
    Credit = 2
}
