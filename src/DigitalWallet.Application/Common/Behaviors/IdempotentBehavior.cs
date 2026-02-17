using System.Reflection;
using DigitalWallet.Application.Common.Interfaces;
using MediatR;

namespace DigitalWallet.Application.Common.Behaviors;
/// <summary>
/// Attribute used to mark a MediatR request as idempotent.
/// When applied, the IdempotencyBehavior will ensure the request
/// is processed only once
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IdempotentAttribute : Attribute
{
    /// <summary>
    /// Indicates where the idempotency key originates (e.g., Header, Body)
    /// Reserved for future use
    /// </summary>
    public string KeySource { get; }
    public IdempotentAttribute(string keySource = "Header")
    {
        KeySource = keySource;
    }

}

/// <summary>
/// MediatR pipeline behavior that enforces idempotency for requests
/// marked with <see cref="cref="IdemptencyAttribute"/> 
/// It prevents duplicate execution by acquiring a lock and returning cached responses for repeated keys.
/// </summary>
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ICurrentUserService _currentUserService;

    public IdempotencyBehavior(IIdempotencyService idempotencyService, ICurrentUserService currentUserService)
    {
        _idempotencyService = idempotencyService;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // ─────────────────────────────────────
        // Step 1 ─ Check if request is marked idempotent
        // ─────────────────────────────────────
        var attribute = typeof(TRequest).GetCustomAttribute<IdempotentAttribute>();
        if (attribute == null)
            return await next();

        // ─────────────────────────────────────
        // Step 2 - Find IdempotencyKey property
        // ─────────────────────────────────────

        var keyProperty = typeof(TRequest).GetProperty("IdempotencyKey");
        if (keyProperty == null || keyProperty.PropertyType != typeof(string))
            return await next();

        var key = keyProperty.GetValue(request)?.ToString();
        if (string.IsNullOrWhiteSpace(key))
            return await next();

        // ─────────────────────────────────────
        // Step 3 ─ Scope key per user
        // ─────────────────────────────────────
        var userId = _currentUserService.UserId ?? "anonymous";
        var fullKey = $"{userId}:{key}";

        var ttl = TimeSpan.FromMinutes(10);

        // ─────────────────────────────────────
        // Step 4 ─ Acquire idempotency lock FIRST
        // ─────────────────────────────────────
        var acquired = await _idempotencyService
            .TryBeginAsync(fullKey, ttl, cancellationToken);

        if (!acquired)
        {
            // Someone else already processed or processing it
            var cached = await _idempotencyService
                .GetCachedResponseAsync<TResponse>(fullKey, cancellationToken);

            if (cached != null)
                return cached;

            throw new InvalidOperationException(
                "Duplicate request detected and no cached response is available");
        }

        // ─────────────────────────────────────
        // Step 5 - Execute handler safety
        // ─────────────────────────────────────
        TResponse response;
        try
        {
            response = await next();
        }
        catch
        {

            throw;
        }

        // ─────────────────────────────────────
        // Step 6 ─ Cache response
        // ─────────────────────────────────────
        await _idempotencyService.CacheResponseAsync(
            fullKey,
            response,
            ttl,
            cancellationToken);


        return response;
    }
}
