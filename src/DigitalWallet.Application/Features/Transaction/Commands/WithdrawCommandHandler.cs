namespace DigitalWallet.Application.Features.Transaction.Commands;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, TransactionDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IKycSubmissionRepository _kycRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TransferDomainService _transferDomainService;
    private readonly IMediator _mediator;
    private readonly ILogger<WithdrawCommandHandler> _logger;

    public WithdrawCommandHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUserRepository userRepository,
        IKycSubmissionRepository kycRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TransferDomainService transferDomainService,
        IMediator mediator,
        ILogger<WithdrawCommandHandler> logger
    )
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _kycRepository = kycRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transferDomainService = transferDomainService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

        var kyc = await _kycRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (kyc?.Status != KycStatus.Approved)
            throw new ForbiddenAccessException("KYC verification required to withdraw.");

        var userAccount = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (userAccount == null)
            throw new NotFoundException(nameof(Account), request.AccountId);

        if (userAccount.UserId != user.Id && !_currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        var systemUser = await _userRepository.GetByEmailAsync("system@internal", cancellationToken);
        if (systemUser == null)
            throw new DomainException("System user not found.");

        var systemPayoutAccount = (await _accountRepository.GetByUserIdAsync(
            systemUser.Id, cancellationToken))
            .FirstOrDefault(a => a.Type == AccountType.FeeIncome && a.Currency.Code == request.CurrencyCode);

        if (systemPayoutAccount == null)
            throw new DomainException($"System payout account for currency {request.CurrencyCode} not found.");

        // Create money object
        var currency = CurrencyRegistry.FromCode(request.CurrencyCode);
        var money = new Money(request.Amount, currency);

        _logger.LogInformation(
            "Starting withdrawal {amount} {currency}...",
            money.Amount, money.Currency);

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            var transaction = _transferDomainService.Transfer(userAccount, systemPayoutAccount, money, request.Description, new IdempotencyKey(request.IdempotencyKey));

            _accountRepository.Update(userAccount);
            _accountRepository.Update(systemPayoutAccount);
            _transactionRepository.Add(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Withdrawal completed successfully {amount}", money.Amount);

            foreach (var domainEvent in transaction.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            transaction.ClearDomainEvents();

            return _mapper.Map<TransactionDto>(transaction);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
