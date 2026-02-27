namespace DigitalWallet.Application.DTOs;

public class KycStatusDto : IMapFrom<KycSubmission>
{
    public Guid UserId { get; set; }
    public KycStatus Status { get; set; } 
    public DateTime SubmittedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? ReviewNotes { get; set; }
}
