using DigitalWallet.Application.Common.Interfaces;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories;
/// <summary>
/// Repository for user entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User?> GetByAuth0IdAsync(string auth0UserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByAuth0IdAsync(string auth0UserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Auth0UserId == auth0UserId, cancellationToken);
    }
    
    public void Add(User user)
    {
        _context.Users.Add(user);
    }



    public void Update(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<User?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        if (account == null) return null;
        return await _context.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == account.Id, cancellationToken);
    }
}
