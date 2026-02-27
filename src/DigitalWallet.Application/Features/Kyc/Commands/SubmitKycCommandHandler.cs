namespace DigitalWallet.Application.Features.Kyc.Commands;
/// <summary>
/// Handles KYC submission: saves document, create/updates KycSubmission entity
/// </summary>
public class SubmitKycCommandHandler : IRequestHandler<SubmitKycCommand, KycStatusDto>
{
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public SubmitKycCommandHandler(
        IKycSubmissionRepository kycRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorageService)
    {
        _kycRepository = kycRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<KycStatusDto> Handle(SubmitKycCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

        var existing = await _kycRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (existing != null && existing.Status != KycStatus.Rejected)
            throw new DomainException("You already have a KYC submission in progress or approved.");

        var filePath = await _fileStorageService.SaveFileAsync(
            request.DocumentFile,
            $"kyc/{user.Id}/{Guid.NewGuid():N}_{request.DocumentFile.FileName}",
            cancellationToken);

        KycSubmission submission;
        if (existing == null)
        {
            submission = new KycSubmission(
                user.Id,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.State,
                request.PostalCode,
                request.Country,
                request.DocumentType,
                request.DocumentNumber,
                filePath);
            _kycRepository.Add(submission);
        }
        else
        {
            await _fileStorageService.DeleteFileAsync(existing.DocumentFilePath, cancellationToken);
            _kycRepository.Remove(existing);
            existing = new KycSubmission(
                user.Id,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.State,
                request.PostalCode,
                request.Country,
                request.DocumentType,
                request.DocumentNumber,
                filePath);
            _kycRepository.Update(existing);
            submission = existing;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KycStatusDto>(submission);
    }
}
