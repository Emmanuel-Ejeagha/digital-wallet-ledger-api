using System;

namespace DigitalWallet.Domain.Exceptions;

public sealed class InsufficientBalanceException : DomainException
{
    public decimal CurrentBalance { get; }
    public decimal RequestedAmount { get; }
    public string Currency { get; }

    public InsufficientBalanceException(
        decimal currentBalance,
        decimal requestedAmount,
        string currency) : base(
            $"Insufficient balance. Current: {currentBalance} {currency}, Requested: {requestedAmount} {currency}",
            $"Your account has insufficient funds to complete this transaction.",
            "INSUFFICIENT_BALANCE")
    {
        CurrentBalance = currentBalance;
        RequestedAmount = requestedAmount;
        Currency = currency;
    }
}
