namespace DigitalWallet.Infrastructure.Entities;

/// <summary>
/// Represents a processed idempotent request.
/// Stores the response of a previously executed command
/// to ensure safe retries and prevent duplicate processing.
/// </summary>
public class IdempotentRequest
{
    /// <summary>
    /// Unique identifier for the idempotent record (UUID for PostgreSQL).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique idempotency key provided by the client.
    /// Must be unique to prevent duplicate command execution.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Serialized JSON response returned for the original request.
    /// Used to replay the exact same response on retries.
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the request was first processed.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// UTC timestamp when this idempotency record expires and can be cleaned up.
    /// </summary>
    public DateTime ExpiredAt { get; set; }
    /// <summary>
    /// Helps avoid race conditions
    /// </summary>
    public bool IsProcessed { get; set; }
    /// <summary>
    /// Prevents key reuse withdifferent payloads (advance, good)
    /// </summary>
    public string RequestHash { get; set; } = string.Empty;
}
