using DigitalWallet.Application.Common.Interfaces;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories;
/// <summary>
/// Repository for Transaction aggregate
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Entries)
                .ThenInclude(e => e.Amount.Currency)
            .Include(t => t.IdempotencyKey)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, DateTime? fromUtc, DateTime? toUtc, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions
            .Where(t => t.Entries
            .Any(e => e.AccountId == accountId));

        if (fromUtc.HasValue)
            query = query.Where(t => t.CreatedAt >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(t => t.CreatedAt <= toUtc.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(t => t.Entries)
                .ThenInclude(e => e.Amount.Currency)
            .ToListAsync(cancellationToken);
    }

    public void Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
    }

    public void Update(Transaction transaction)
    {
        _context.Entry(transaction).State = EntityState.Modified;
    }
}
