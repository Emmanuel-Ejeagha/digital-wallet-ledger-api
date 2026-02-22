using System;
using DigitalWallet.Application.Common.Interfaces;
using DigitalWallet.Application.Features.Transaction.Notifications;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.Features.Transaction.EventHandlers;
/// <summary>
/// Purpose: Handles MoneyTransferredEvent and sends email notifications to both parties.
/// </summary>
public class MoneyTransferredNotificationHandler : INotificationHandler<MoneyTransferredNotification>
{
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<MoneyTransferredNotificationHandler> _logger;

    public MoneyTransferredNotificationHandler(
        IEmailService emailService,
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ILogger<MoneyTransferredNotificationHandler> logger)
    {
        _emailService = emailService;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task Handle(MoneyTransferredNotification notification, CancellationToken cancellationToken)
    {
        var transaction = notification.DomainEvent.Transaction;
        if (transaction.Entries.Count < 2)
        {
            _logger.LogWarning(
                "Transaction {TransactionId} has insufficient entries for transfer email.",
                transaction.Id);
            return;
        }

        var creditEntry = transaction.Entries.FirstOrDefault(e => e.Type == EntryType.Credit);
        var debitEntry = transaction.Entries.FirstOrDefault(e => e.Type == EntryType.Debit);

        if (creditEntry == null || debitEntry == null)
        {
            _logger.LogError(
                "Transaction {TransactionId} missing credit/debit entries.",
                transaction.Id);
            return;
        }

        // Get accounts
        var senderAccount = await _accountRepository.GetByIdAsync(creditEntry.AccountId, cancellationToken);
        var receiverAccount = await _accountRepository.GetByIdAsync(debitEntry.AccountId, cancellationToken);

        if (senderAccount == null || receiverAccount == null)
        {
            _logger.LogError(
                "Accounts not found for transaction {TransactionId}",
                transaction.Id);
            return;
        }

        // Get users (single source of truth)
        var senderUser = await _userRepository.GetByIdAsync(senderAccount.UserId, cancellationToken);
        var receiverUser = await _userRepository.GetByIdAsync(receiverAccount.UserId, cancellationToken);

        if (senderUser == null || receiverUser == null)
        {
            _logger.LogError(
                "Users not found for transaction {TransactionId}",
                transaction.Id);
            return;
        }

        var amount = creditEntry.Amount;
        var formattedAmount = $"{amount.Currency.Symbol}{amount.Amount.ToString($"F{amount.Currency.DecimalPlaces}")}";

        // Sender Email (Debit)
        var senderSubject = $"Transfer Sent - {formattedAmount}";
        var senderBody = $@"
            <h2>Transfer Successful</h2>
            <p>Dear {senderUser.FirstName},</p>
            <p>You have successfully sent <strong>{formattedAmount}</strong> to {receiverUser.FirstName} {receiverUser.LastName}.</p>
            <p><strong>Transaction Reference:</strong> {transaction.Reference}</p>
            <p><strong>Date:</strong> {transaction.CompletedAt:yyyy-MM-dd HH:mm:ss} UTC</p>
        ";

        // Receiver Email (Credit)
        var receiverSubject = $"Transfer Received - {formattedAmount}";
        var receiverBody = $@"
            <h2>Funds Received</h2>
            <p>Dear {receiverUser.FirstName},</p>
            <p>You have received <strong>{formattedAmount}</strong> from {senderUser.FirstName} {senderUser.LastName}.</p>
            <p><strong>Transaction Reference:</strong> {transaction.Reference}</p>
            <p><strong>Date:</strong> {transaction.CompletedAt:yyyy-MM-dd HH:mm:ss} UTC</p>
        ";

        await _emailService.SendEmailAsync(senderUser.Email, senderSubject, senderBody, cancellationToken);
        await _emailService.SendEmailAsync(receiverUser.Email, receiverSubject, receiverBody, cancellationToken);

        _logger.LogInformation(
            "Transfer email notifications sent for transaction {TransactionId}",
            transaction.Id);
    }
}
