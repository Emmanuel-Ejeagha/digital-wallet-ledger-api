namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Encapsulates a database transaction scope. Used to commit multiple changes atomically
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persists all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation</param>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    ///     /// <param name="cancellationToken">Token used to cancel the operation</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

       /// <summary>
    /// Begins a new database transaction
    /// </summary>
    ///     /// <param name="cancellationToken">Token used to cancel the operation</param>
///  <param name="isolationLevel">TSpecifies the transaction locking behavior for the connection.</param>
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

/// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Roll back the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
