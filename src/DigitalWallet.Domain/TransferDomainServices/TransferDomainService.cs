using System;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.TransferDomainServices
{
    public class TransferDomainService
    {
        public Transaction Transfer(Account fromAccount, Account toAccount, Money amount, string description, IdempotencyKey idempotencyKey)
        {
            if (fromAccount == null) throw new ArgumentNullException(nameof(fromAccount));
            if (toAccount == null) throw new ArgumentNullException(nameof(toAccount));
            if (amount == null) throw new ArgumentNullException(nameof(amount));
            if (idempotencyKey == null) throw new ArgumentNullException(nameof(idempotencyKey));

            if (!fromAccount.IsActive)
                throw new DomainException("Source account is not active.");
            if (!toAccount.IsActive)
                throw new DomainException("Destination account is not active.");

            if (fromAccount.Currency != amount.Currency)
                throw new DomainException($"Source account currency ({fromAccount.Currency.Code}) does not match transfer currency ({amount.Currency.Code}).");
            if (toAccount.Currency != amount.Currency)
                throw new DomainException($"Destination account currency ({toAccount.Currency.Code}) does not match transfer currency ({amount.Currency.Code}).");

            if (fromAccount.Balance < amount.Amount)
                throw new DomainException("Insufficient funds.");

            // Fixed prefix from "TNX-" to "TXN-"
            var reference = $"TXN-{Guid.NewGuid():N}";

            var transaction = new Transaction(reference, description, idempotencyKey);

            var creditEntry = new LedgerEntry(
                accountId: fromAccount.Id,
                transactionId: transaction.Id,
                type: EntryType.Credit,
                amount: amount,
                balanceAfter: fromAccount.Balance - amount.Amount,
                description: $"Transfer to {toAccount.Name}"
            );

            var debitEntry = new LedgerEntry(
                accountId: toAccount.Id,
                transactionId: transaction.Id,
                type: EntryType.Debit,
                amount: amount,
                balanceAfter: toAccount.Balance + amount.Amount,
                description: $"Transfer from {fromAccount.Name}"
            );

            fromAccount.ApplyCredit(amount);
            toAccount.ApplyDebit(amount);

            transaction.AddEntry(creditEntry);
            transaction.AddEntry(debitEntry);

            transaction.Complete();

            return transaction;
        }
    }
}