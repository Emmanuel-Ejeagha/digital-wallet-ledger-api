namespace DigitalWallet.Application.Features.Kyc.Commands;
/// <summary>
/// Command to submit KYC information with document upload.
/// </summary>
public class SubmitKycCommand : IRequest<KycStatusDto>
{
    public string IdempotencyKey { get; set; } = string.Empty;

    // Personal details
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public IFormFile DocumentFile { get; set; } = null!;

}
