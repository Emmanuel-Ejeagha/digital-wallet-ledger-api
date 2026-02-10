using MediatR;
using Microsoft.EntityFrameworkCore;
using DigitalWallet.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior for transaction management
    /// CRITICAL: Ensures financial operations are atomic
    /// </summary>
    public sealed class TransactionBehavior<TRequest, TResponse> 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(
            IApplicationDbContext dbContext,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var requestName = typeof(TRequest).Name;

            try
            {
                // Check if request implements ITransactionalRequest
                if (request is not ITransactionalRequest)
                {
                    // Not transactional - just execute
                    return await next();
                }

                // Begin transaction with serializable isolation for financial operations
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _dbContext.Database
                        .BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);

                    _logger.LogInformation(
                        "Begin transaction {TransactionId} for {RequestName}",
                        transaction.TransactionId,
                        requestName);

                    try
                    {
                        response = await next();
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation(
                            "Committed transaction {TransactionId} for {RequestName}",
                            transaction.TransactionId,
                            requestName);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        
                        _logger.LogError(
                            "Rolled back transaction {TransactionId} for {RequestName}",
                            transaction.TransactionId,
                            requestName);
                        
                        throw;
                    }
                });

                return response!;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling transaction for {RequestName}",
                    requestName);
                
                throw;
            }
        }
    }

    /// <summary>
    /// Marker interface for requests that require transactions
    /// </summary>
    public interface ITransactionalRequest { }
}