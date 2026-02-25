namespace DigitalWallet.Infrastructure.Repositories;
/// <summary>
/// Repository for Account aggregate
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Currency)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Currency)
            .Where(a => a.UserId == userId) 
            .ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetByUserAndCurrencyAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Currency)
            .FirstOrDefaultAsync(a =>
                a.UserId == userId &&
                a.Currency.Code == currencyCode,
                cancellationToken);
            
    }
    
    public async Task<Account?> GetSystemReservedAccountByCurrencyAsync(
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Currency)
            .FirstOrDefaultAsync(a =>
                a.Type == AccountType.SystemReserve &&
                a.Currency.Code == currencyCode,
                cancellationToken);
    }

    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }
    public void Remove(Account account)
    {
        _context.Accounts.Remove(account);
    }

}
