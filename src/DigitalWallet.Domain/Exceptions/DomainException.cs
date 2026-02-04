using System;

namespace DigitalWallet.Domain.Exceptions;
/// <summary>
/// Base exception for all domain-related errors
/// CRITICAL: Never expose sensitive financial data in exception messages
/// </summary>
public class DomainException : Exception
{
    public string ErrorCode { get; }
    public string UserMessage { get; }

    public DomainException(string message, string errorCode = "DOMAIN_ERROR") : base(message)
    {
        ErrorCode = errorCode;
        UserMessage = "An error occurred while processing your request";
    }

    public DomainException(string message, Exception innerException, string errorCode = "DOMAIN_ERROR") : base(message, innerException)
    {
        ErrorCode = errorCode;
        UserMessage = "An error occurred while processing your request.";
    }

    protected DomainException(string message, string userMessage, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
        UserMessage = userMessage;
    }
}
