namespace DigitalWallet.Infrastructure.Persistence;
/// <summary>
/// Entity Framework Core database context for the ledger system.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<KycSubmission> KycSubmissions => Set<KycSubmission>();
    public DbSet<IdempotentRequest> IdempotentRequests => Set<IdempotentRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
