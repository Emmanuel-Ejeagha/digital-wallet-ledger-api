namespace DigitalWallet.Application.Features.Admin.Queries;

public class GetKycSubmissionQuery : IRequest<IEnumerable<KycSubmissionDto>>
{
    public KycStatus? Status { get; set; }
}
