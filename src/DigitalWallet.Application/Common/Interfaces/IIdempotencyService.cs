namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Service to manage idempotency keys. Ensures that a request with the same key is processed only once.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
        /// Attempts to begin processing a request with the given key.
        /// Returns true if this instance should process the request (i.e., it acquired the lock).
        /// </summary>
        /// <param name="key">Idempotency key.</param>
        /// <param name="ttl">Time-to-live for the request lock.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the caller should process the request; false if already processed or in progress.</returns>
        Task<bool> TryBeginAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the request hash for a given key to detect key reuse with different payloads.
        /// </summary>
        Task SetRequestHashAsync(string key, string requestHash, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a cached response for the given key, if it exists and has not expired.
        /// </summary>
        Task<TResponse?> GetCachedResponseAsync<TResponse>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Caches a response for the given key with a TTL.
        /// </summary>
        Task CacheResponseAsync<TResponse>(string key, TResponse response, TimeSpan? ttl = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a key exists and has been processed (and not expired).
        /// </summary>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
