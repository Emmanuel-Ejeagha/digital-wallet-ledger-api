namespace DigitalWallet.Application.Features.Transaction.Commands;
/// <summary>
/// Handles deposits by transferring from system reserve account to user account
/// </summary>
public class DepositCommandHandler : IRequestHandler<DepositCommand, TransactionDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TransferDomainService _transferDomainService;
    private readonly IMediator _mediator;
    private readonly ILogger<DepositCommandHandler> _logger;
    public DepositCommandHandler(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TransferDomainService transferDomainService,
        IMediator mediator,
        ILogger<DepositCommandHandler> logger


    )
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transferDomainService = transferDomainService;
        _mediator = mediator;
        _logger = logger;        
    }

    public async Task<TransactionDto> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting deposit for AccountId: {AccountId}, Amount: {Amount} {Currency}",
            request.AccountId, request.Amount, request.CurrencyCode);

        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var userId = _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User ID is missing from context.");
        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);

        if (user == null)
            throw new NotFoundException("User", userId);

        var userAccount = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (userAccount == null)
            throw new NotFoundException(nameof(Account), request.AccountId);

        if (userAccount.UserId != user!.Id && !_currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        var systemReservedAccount = await _accountRepository
            .GetSystemReservedAccountByCurrencyAsync(request.CurrencyCode, cancellationToken);

        if (systemReservedAccount == null)
            throw new DomainException($"System reserve account for currency {request.CurrencyCode} not found");

        var currency = CurrencyRegistry.FromCode(request.CurrencyCode);
        var money = new Money(request.Amount, currency);

        await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);

        try
        {
            var transaction = _transferDomainService.Transfer(
                systemReservedAccount,
                userAccount,
                money,
                request.Description,
                new IdempotencyKey(request.Idempotency));

            _accountRepository.Update(systemReservedAccount);
            _accountRepository.Update(userAccount);
            _transactionRepository.Add(transaction);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var domainEvents = transaction.DomainEvents.ToList();
            transaction.ClearDomainEvents();

            foreach (var domainEvent in transaction.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            _logger.LogInformation("Deposit completed successfully. TransactionId: {TransactionId}", transaction.Id);

            return _mapper.Map<TransactionDto>(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Deposit failed for AccountId: {AccountId}", request.AccountId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
