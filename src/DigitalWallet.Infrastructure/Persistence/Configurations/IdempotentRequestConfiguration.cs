namespace DigitalWallet.Infrastructure.Persistence.Configurations;

public class IdempotentRequestConfiguration : IEntityTypeConfiguration<IdempotentRequest>
{
    public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
    {
        builder.ToTable("IdempotentRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Key)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.RequestHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.IsProcessed)
            .IsRequired();

        builder.Property(r => r.Response)
            .IsRequired()
            .HasColumnType("jsonb"); // Store response as JSON

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();
        
        // Concurrency token for optimistic concurrency
            builder.Property(r => r.ConcurrencyToken)
                .IsRowVersion()
                .HasColumnName("xmin");

        // Indexes
        // Unique constraint - CRITICAL for idempotency safety
        builder.HasIndex(r => r.Key)
            .IsUnique()
            .HasDatabaseName("IX_IdempotentRequests_Key");

        // Prevent same key with different request body
        builder.HasIndex(x => new { x.Key, x.RequestHash })
            .HasDatabaseName("IX_IdempotentRequests_Key_RequestHash");

        // Expiry cleanup performance
        builder.HasIndex(r => r.ExpiresAt)
            .HasDatabaseName("IX_IdempotentRequests_ExpiredAt");

    }
}
