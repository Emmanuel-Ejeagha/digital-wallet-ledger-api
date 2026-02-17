using System;
using System.Diagnostics;
using DigitalWallet.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.Common.Behaviors;
/// <summary>Logs request execution, duration, and failures.</summary>
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? "Anonymous";

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "➡️ Handling {RequestName} for User {UserId}",
            requestName,
            userId);
        
        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "✅ Handled {RequestName} for User {UserId} in {ElapsedMs} ms",
                requestName,
                userId,
                stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "❌ Error handling {RequestName} for User {UserId} for {ElapedMs} ms",
                requestName,
                userId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }

    }
}
