namespace DigitalWallet.Application.Features.Accounts.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IKycSubmissionRepository _kycRepository;
    private ILogger<CreateAccountCommandHandler> _logger;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IKycSubmissionRepository kycRepository,
        ILogger<CreateAccountCommandHandler> logger
    )
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _kycRepository = kycRepository;
        _logger = logger;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);

        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

        var kyc = await _kycRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (kyc?.Status != KycStatus.Approved)
            throw new ForbiddenAccessException("KYC verification required to create an account");

        var currency = CurrencyRegistry.FromCode(request.CurrencyCode);

        var existing = await _accountRepository.GetByUserAndCurrencyAsync(user.Id, request.CurrencyCode, cancellationToken);

        if (existing != null)
            throw new DomainException($"User already has an account in currency {request.CurrencyCode}.");

        if (!Enum.TryParse<AccountType>(
                request.AccountType, true, out var accountType))
            throw new DomainException(
                $"Invalid account type: {request.AccountType}");

        var account = new Account(
            user.Id,
            accountType,
            currency,
            request.Name);

        _accountRepository.Add(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Account {AccountId} created for user {UserId}",
            account.Id,
            user.Id);

        return _mapper.Map<AccountDto>(account);
    }
}

