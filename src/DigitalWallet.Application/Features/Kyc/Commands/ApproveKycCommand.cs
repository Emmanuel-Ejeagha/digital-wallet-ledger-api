using System;

namespace DigitalWallet.Application.Features.Kyc.Commands;

public class ApproveKycCommand : IRequest
{
    public Guid SubmissionId { get; set; }
}

public class ApproveKycCommandHandler : IRequestHandler<ApproveKycCommand>
{
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveKycCommandHandler(
        IKycSubmissionRepository kycRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _kycRepository = kycRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ApproveKycCommand request, CancellationToken cancellationToken)
    {
        var submission = await _kycRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission == null)
            throw new NotFoundException(nameof(KycSubmission), request.SubmissionId);

        submission.Approve(_currentUserService.UserId!);
        _kycRepository.Update(submission);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
