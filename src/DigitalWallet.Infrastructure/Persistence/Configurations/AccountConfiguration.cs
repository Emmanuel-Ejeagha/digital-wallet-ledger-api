using System;
using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configures the Acount entity for EF Core.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50); // Store enum as string

        // Currency as owned type
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

        builder.Property(a => a.Balance)
            .IsRequired()
            .HasColumnType("numeric(19,4)");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.ConcurrencyToken)
            .IsRowVersion()
            .HasColumnName("xmin");

        // Relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => new { a.UserId, a.Currency })
            .IsUnique()
            .HasDatabaseName("IX_Accounts_UserId_Currency");

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("IX_Accounts_UserId");
    }
}
