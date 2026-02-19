using DigitalWallet.Domain.Entities;
using DigitalWallet.Infrastructure.Entities;
using DigitalWallet.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Persistence;
/// <summary>
/// Entity Framework Core database context for the ledger system.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    // DbSets for aggregates
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<IdempotentRequest> IdempotentRequests => Set<IdempotentRequest>(); // For idempotency

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new LedgerEntryConfiguration());
        modelBuilder.ApplyConfiguration(new IdempotentRequestConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
