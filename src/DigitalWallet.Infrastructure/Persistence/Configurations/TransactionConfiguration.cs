namespace DigitalWallet.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configures the Transaction Entity for EF Core
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Reference)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.CompletedAt);

        builder.OwnsOne(t => t.IdempotencyKey, key =>
        {
            key.Property(k => k.Value)
                .HasColumnName("IdempotencyKeyValue")
                .IsRequired()
                .HasMaxLength(100);

            key.HasIndex(k => k.Value)
                .IsUnique()
                .HasDatabaseName("IX_Transactions_Idempotency");

            key.WithOwner();
        });

        builder.HasMany(t => t.Entries)
            .WithOne()
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Reference)
        .IsUnique()
        .HasDatabaseName("IX_Transactions_Reference");
    }
}
