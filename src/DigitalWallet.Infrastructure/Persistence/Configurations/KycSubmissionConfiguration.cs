namespace DigitalWallet.Infrastructure.Persistence.Configurations;
/// <summary>
/// EF Core configuration for KycSubmission etity.
/// </summary>
public class KycSubmissionConfiguration : IEntityTypeConfiguration<KycSubmission>
{
    public void Configure(EntityTypeBuilder<KycSubmission> builder)
    {
        builder.ToTable("KycSubmissions");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.UserId)
            .IsRequired();

        builder.Property(k => k.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.DateOfBirth)
            .IsRequired();

        builder.Property(k => k.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(k => k.AddressLine2)
            .HasMaxLength(200);

        builder.Property(k => k.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.State)
            .HasMaxLength(100);

        builder.Property(k => k.PostalCode)
            .HasMaxLength(100);

        builder.Property(k => k.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.DocumentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(k => k.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(k => k.DocumentFilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(k => k.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(k => k.SubmittedAt)
            .IsRequired();

        builder.Property(k => k.ReviewedAt);

        builder.Property(k => k.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(k => k.ReviewedBy)
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(k => k.UserId)
            .IsUnique()
            .HasDatabaseName("IX_KycSubmissions_UserId");

        builder.HasIndex(k => k.Status)
            .HasDatabaseName("IX_KycSubmissions_Status");        
    }
}
