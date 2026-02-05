using System;

namespace DigitalWallet.Domain.Exceptions;
/// <summary>
/// CRITICAL: Thrown when transaction violates financial rules
/// Ensures double-entry accounting invariants are maintained
/// </summary>
public sealed class InvalidTransactionException : DomainException
{
    public InvalidTransactionException(string message)
        : base(message, "The transaction could not be processed due to validation errors.", "INVALID_TRANSACTION") { }

    public InvalidTransactionException(string message, Exception innerException)
        : base(message, innerException, "INVALID_TRANSACTION") { }
}
