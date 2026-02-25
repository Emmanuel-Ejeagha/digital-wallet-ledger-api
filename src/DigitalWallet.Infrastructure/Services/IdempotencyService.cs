namespace DigitalWallet.Infrastructure.Services;
/// <summary>
/// Idempotency service using a database table. Responses are stored as JSON with a TTL(Time to live)
/// </summary>
public class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly TimeSpan _defaultTtl = TimeSpan.FromHours(24);

    public IdempotencyService(ApplicationDbContext context, IDateTime dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    /// <summary>
    /// Attempts to begin processing an idempotent request.
    /// If the key does not exist, a new record is inserted and returns true.
    /// If the key exists and already processed, returns false (response should be retrieved separately).
    /// If the key exists but is not yet processed (concurrent request), return false (caller should wait/retry).
    /// </summary>
    /// <param key="name">Idempotency key</param>
    /// <param name="ttl">Time-to-live for the request record</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if this instance acquired the lock and should process; otherwise false.</returns>
    public async Task<bool> TryBeginAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        // Hash the key? No, key is already unique per user
        var now = _dateTime.UtcNow;
        var expiresAt = now.Add(ttl);

        
        await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);

        var existing = await _context.IdempotentRequests
            .FirstOrDefaultAsync(r => r.Key == key, cancellationToken);

        if (existing == null)
        {
            // No record - we are the first. Insert and commit.
            var request = new IdempotentRequest
            {
                Id = Guid.NewGuid(),
                Key = key,
                RequestHash = string.Empty, // Will be set later when we have the payload
                IsProcessed = false,
                CreatedAt = now,
                ExpiresAt = expiresAt
            };
            _context.IdempotentRequests.Add(request);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        // Record exists.
        if (existing.IsProcessed)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        // Record exists but not processed - another request is in progress.
        // We could optionally check if the request has expired and allow retry
        if (existing.ExpiresAt < now)
        {
            _context.IdempotentRequests.Remove(existing);
            var request = new IdempotentRequest
            {
                Id = Guid.NewGuid(),
                Key = key,
                RequestHash = string.Empty,
                IsProcessed = false,
                CreatedAt = now,
                ExpiresAt = expiresAt
            };
            _context.IdempotentRequests.Add(request);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        // still within TTL and not processed - another request is active.
        await transaction.RollbackAsync(cancellationToken);
        return false;
    }

    /// <summary>
    ///  Sores the request hash, marks request as processed, and caches a response for the given key with TTL.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="key">Idempotency key</param>
    /// <param name="response">Response to cache.</param>
    /// <param name="">Optional custom TTL. if not provided, default 24h is used.</param>
    public async Task CacheResponseAsync<TResponse>(string key, TResponse response, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        var request = await _context.IdempotentRequests.FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
        if (request == null)
        {
            // Should not happen if TryBegin was called first, but handle gracefully
            request = new IdempotentRequest
            {
                Id = Guid.NewGuid(),
                Key = key,
                CreatedAt = _dateTime.UtcNow,
                ExpiresAt = _dateTime.UtcNow.Add(ttl ?? _defaultTtl)
            };
            _context.IdempotentRequests.Add(request);
        }

        request.Response = JsonSerializer.Serialize(response);
        request.IsProcessed = true;
        request.ExpiresAt = _dateTime.UtcNow.Add(ttl ?? _defaultTtl);

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Sets the request hash for a given key (to validate payload on subsequent attempts).
    /// </summary>
    public async Task SetRequestHashAsync(string key, string requestHash, CancellationToken cancellationToken = default)
    {
        var request = await _context.IdempotentRequests.FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
        if (request != null)
        {
            request.RequestHash = requestHash;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves a cached response for the given key, if it exists and has not expired.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="key">Idempotency key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached response, or default if not found/expired.</returns>
    public async Task<TResponse?> GetCachedResponseAsync<TResponse>(string key, CancellationToken cancellationToken = default)
    {
        var request = await _context.IdempotentRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Key == key && r.IsProcessed && r.ExpiresAt > _dateTime.UtcNow, cancellationToken);

        if (request == null)
            return default;

        return JsonSerializer.Deserialize<TResponse>(request.Response);
    }

    /// <summary>
    /// Checks if a key exists and is processed (and not expired).
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.IdempotentRequests
         .AnyAsync(r => r.Key == key && r.IsProcessed && r.ExpiresAt > _dateTime.UtcNow, cancellationToken);
    }
}
