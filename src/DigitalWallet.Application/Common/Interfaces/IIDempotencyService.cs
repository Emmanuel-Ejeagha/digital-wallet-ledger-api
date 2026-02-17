namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Service to manage idempotency keys. Ensures that a request with the same key is processed only once.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Attempts to atomically register the specified idempotency key as "in progress".
    /// Returns true if the key was successfully reserved and processing may proceed,
    /// or false if the key already exists or is currently being processed.
    /// </summary>
    Task<bool> TryBeginAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
    /// <summary>
    /// Checks if a key has already been processed and returns the cached response if any.
    /// </summary>
    Task<TResponse?> GetCachedResponseAsync<TResponse>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a key as processed and caches the response.
    /// </summary>
    Task CacheResponseAsync<TResponse>(string key, TResponse response, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists (already processed).
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
