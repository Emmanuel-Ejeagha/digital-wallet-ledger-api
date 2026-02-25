namespace DigitalWallet.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>();

        // Amount as owned type
        builder.OwnsOne(e => e.Amount, amount =>
        {
            amount.Property(a => a.Amount)
            .HasColumnName("Amount")
            .IsRequired()
            .HasPrecision(18, 6);

            amount.OwnsOne(a => a.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("CurrencyCode")
                    .IsRequired()
                    .HasMaxLength(3);

                currency.Property(c => c.DecimalPlaces)
                    .HasColumnName("CurrencyDecimalPlaces");

                currency.Property(c => c.Symbol)
                    .HasColumnName("CurrencySymbol")
                    .HasMaxLength(10);

                currency.Property(c => c.Name)
                    .HasColumnName("CurrencyName")
                    .HasMaxLength(50);
            });

            amount.WithOwner();
        });

        builder.Property(e => e.BalanceAfter)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(e => e.AccountId)
            .HasDatabaseName("IX_LedgerEntries_AccountId");

        builder.HasIndex(e => e.TransactionId)
            .HasDatabaseName("IX_LedgerEntries_TransactionId");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_LedgerEntries_CreatedAt");
    }
}
