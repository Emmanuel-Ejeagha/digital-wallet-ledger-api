namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Aggregate root for KYC submissions. Each user can have at most one submission.
/// </summary>
public class KycSubmission : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public string DocumentType { get; private set; }
    public string DocumentNumber { get; private set; }
    public string DocumentFilePath { get; private set; }
    public KycStatus Status { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }
    public string? ReviewedBy { get; private set; }

    private KycSubmission() { }

    public KycSubmission(
        Guid userId,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country,
        string documentType,
        string documentNumber,
        string documentFilePath) : base()
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name required", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name required", nameof(lastName));
        if (dateOfBirth > DateTime.UtcNow.AddYears(-18)) throw new DomainException("User must be at least 18 years old.");
        if (string.IsNullOrWhiteSpace(addressLine1)) throw new ArgumentException("Address required", nameof(addressLine1));
        if (string.IsNullOrWhiteSpace(documentType)) throw new ArgumentException("Document number required", nameof(documentType));
        if (string.IsNullOrWhiteSpace(documentNumber)) throw new ArgumentException("Document number required", nameof(documentNumber));
        if (string.IsNullOrWhiteSpace(documentFilePath))
        throw new ArgumentException("Document number number required", nameof(documentFilePath));

        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2 ?? string.Empty;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        Status = KycStatus.Pending;
        SubmittedAt = SubmittedAt;
    }

    public void Approve(string reviewer)
    {
        if (Status != KycStatus.Pending && Status != KycStatus.UnderReview)
            throw new DomainException("Only pending or under review submissions can be approved.");
        Status = KycStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewer;
    }

    public void Reject(string reviewer, string notes)
    {
        if (Status != KycStatus.Pending && Status != KycStatus.UnderReview)
            throw new DomainException("only pending or under review submissions can be rejected.");
        Status = KycStatus.Rejected;
        ReviewedAt = DateTime.UtcNow;
        ReviewedBy = reviewer;
        ReviewNotes = notes;
    }

    public void MarkUnderReview()
    {
        if (Status != KycStatus.Pending)
            throw new DomainException("Only pending submissions can be marked under review.");
        Status = KycStatus.UnderReview;
    }

}
