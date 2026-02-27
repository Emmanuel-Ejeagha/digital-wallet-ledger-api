using System;

namespace DigitalWallet.Application.Features.Kyc.Queries;

public class GetKycStatusQueryHandler : IRequestHandler<GetKycStatusQuery, KycStatusDto>
{
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetKycStatusQueryHandler(
        IKycSubmissionRepository kycRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _kycRepository = kycRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<KycStatusDto> Handle(GetKycStatusQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(user), _currentUserService.UserId!);

        var submission = await _kycRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (submission == null)
        {
            return new KycStatusDto { Status = KycStatus.NotSubmitted };
        }

        return _mapper.Map<KycStatusDto>(submission);
    }
}
