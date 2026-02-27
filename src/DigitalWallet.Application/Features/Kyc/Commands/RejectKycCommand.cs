namespace DigitalWallet.Application.Features.Kyc.Commands;

public class RejectKycCommand : IRequest
{
    public Guid SubmissionId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class RejectKycCommandHandler : IRequestHandler<RejectKycCommand>
{
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectKycCommandHandler(
        IKycSubmissionRepository kycRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _kycRepository = kycRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RejectKycCommand request, CancellationToken cancellationToken)
    {
        var submission = await _kycRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission == null)
            throw new NotFoundException(nameof(KycSubmission), request.SubmissionId);

        submission.Reject(_currentUserService.UserId!, request.Notes);
        _kycRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
