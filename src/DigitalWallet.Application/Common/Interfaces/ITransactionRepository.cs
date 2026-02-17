using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>Repository for Transaction aggregate</summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Gets a transaction by its identifier.
    /// </summary>
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paged transactions for an account within an optionsl date range
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(
        Guid accountId,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new transaction aggregate.
    /// </summary>
    void Add(Transaction transaction);
    
    /// <summary>
    /// Marks a transaction aggregate as updated.
    /// </summary>
    void Update(Transaction transaction);
}
