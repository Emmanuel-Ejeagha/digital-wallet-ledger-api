namespace DigitalWallet.Application.Features.Kyc.Queries;

public class GetKycSubmissionsQuery : IRequest<List<KycSubmissionDto>>
{
    public KycStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetKycSubmissionsQueryHandler : IRequestHandler<GetKycSubmissionsQuery, List<KycSubmissionDto>>
{
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly IMapper _mapper;

    public GetKycSubmissionsQueryHandler(
        IKycSubmissionRepository keyRepository, IMapper mapper)
    {
        _kycRepository = keyRepository;
        _mapper = mapper;
    }

    public async Task<List<KycSubmissionDto>> Handle(GetKycSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var submission = await _kycRepository.GetAllAsync(request.Status, request.Page, request.Page, cancellationToken);
        return _mapper.Map<List<KycSubmissionDto>>(submission);
    }
}
