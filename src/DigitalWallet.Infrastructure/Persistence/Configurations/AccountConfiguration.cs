namespace DigitalWallet.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the Account entity for EF Core.
    /// </summary>
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Ignore(a => a.CurrencyCode);
            // UserId
            builder.Property(a => a.UserId)
                .IsRequired();

            // Account Type (Enum as string)
            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            // Owned Currency configuration (Value Object)
            builder.OwnsOne(a => a.Currency, currency =>
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

                currency.WithOwner();
            });

            // Balance
            builder.Property(a => a.Balance)
                .IsRequired()
                .HasColumnType("numeric(19,4)");

            // Name
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Active flag
            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Concurrency Token (PostgreSQL xmin or RowVersion)
            builder.Property(a => a.ConcurrencyToken)
                .IsRowVersion()
                .HasColumnName("xmin");

            // Relationship with User
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ CORRECT index for owned type (strongly typed)
            builder.HasIndex("UserId", "CurrencyCode")
                .IsUnique()
                .HasDatabaseName("IX_Accounts_UserId_CurrencyCode");

            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_Accounts_UserId");
        }
    }
}
