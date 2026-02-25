namespace DigitalWallet.Domain.TransferDomainServices;
/// <summary>
/// Domain service that orchestrates a transfer between two accounts.
/// It creates the transaction, adds ledger entries, updates balances, and completes the transaction
/// </summary>
public class TransferDomainService
{
    /// <summary>
    /// Executes a transfer from one account to the another
    /// </summary>
    /// <param name="fromAccount">Source account (will be credited).</param>
    /// <param name="toAccount">Destination account (will be debited).</param>
    /// <param name="amount">Amount to transfer (must be positive).</param>
    /// <param name="description">Description for the transaction.</param>
    /// <param name="idempotencyKey">UIdempotency key for the transfer.</param>
    /// <returns>The created transaction (in Completed state).</returns>
    public Transaction Transfer(Account fromAccount, Account toAccount, Money amount, string description, IdempotencyKey idempotencyKey)
    {
        if (fromAccount == null) throw new ArgumentNullException(nameof(fromAccount));

        if (toAccount == null) throw new ArgumentNullException(nameof(toAccount));

        if (amount == null) throw new ArgumentNullException(nameof(amount));

        if (idempotencyKey == null) throw new ArgumentNullException(nameof(idempotencyKey));

        // Both accounts must be active and same currency (or allow cross-currency? We'll keep same currency for now)
        if (!fromAccount.IsActive)
            throw new DomainException("Source account is not active");

        if (!toAccount.IsActive)
            throw new DomainException("Destination account is not active.");

        if (fromAccount.Currency != amount.Currency)
            throw new DomainException($"Source account currency ({fromAccount.Currency.Code}) does not match transfer currency ({amount.Currency.Code}).");

        if (toAccount.Currency != amount.Currency)
            throw new DomainException($"Destination account currency ({toAccount.Currency.Code}) does not match transfer currency ({amount.Currency.Code}).");

        if (fromAccount.Balance < amount.Amount)
            throw new DomainException("Insufficient funds.");

        // Generate a unique reference for the transaction (could be a GUID or timestamp-based ID)
        var reference = $"TNX-{Guid.NewGuid():N}";

        // Create the transaction aggregate
        var transaction = new Transaction(reference, description, idempotencyKey);

        // Create ledger entries (debit to destination, credit to source)
        // Transfer from A to B: A's balance decreases (credit), B's balance increases (debit).
        var creditEntry = new LedgerEntry(
            accountId: fromAccount.Id,
            transactionId: transaction.Id,
            type: EntryType.Credit,
            amount: amount,
            balanceAfter: fromAccount.Balance - amount.Amount, // after credit
            description: $"Transfer to {toAccount.Name}"
        );

        var debitEntry = new LedgerEntry(
            accountId: toAccount.Id,
            transactionId: transaction.Id,
            type: EntryType.Debit,
            amount: amount,
            balanceAfter: toAccount.Balance + amount.Amount, // after debit
            description: $"Transfer from {fromAccount.Name}"
        );

        // Apply the changes to account bal;ances (domain logic inside Account)
        fromAccount.ApplyCredit(amount);
        toAccount.ApplyDebit(amount);

        // Add entries to transaction
        transaction.AddEntry(creditEntry);
        transaction.AddEntry(debitEntry);

        // Complete the transaction (checks debits == credits)
        transaction.Complete();

        return transaction;
    }
}
