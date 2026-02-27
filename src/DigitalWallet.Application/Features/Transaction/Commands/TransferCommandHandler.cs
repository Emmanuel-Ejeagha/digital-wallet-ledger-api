using ValidationException = DigitalWallet.Application.Common.Exceptions.ValidationException;
namespace DigitalWallet.Application.Features.Transaction.Commands;

/// <summary>
/// Command to transfer money between two accounts.
/// </summary>
public class TransferCommandHandler : IRequestHandler<TransferCommand, TransactionDto>
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
    private readonly ILogger<TransferCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransferCommandHandler"/> class.
    /// </summary>
    /// <param name="accountRepository">Repository used to load and update account aggregates.</param>
    /// <param name="transactionRepository">Repository used to persist transaction aggregates.</param>
    /// <param name="currentUserService">Service that provides information about the currently authenticated user.</param>
    /// <param name="unitOfWork">Unit of work used to manage database transactions and persistence.</param>
    /// <param name="mapper">AutoMapper instance used to map domain entities to DTOs.</param>
    /// <param name="transferDomainService">Domain service that encapsulates transfer business rules and ledger creation.</param>
    /// <param name="logger">Logger for structured transfer operation logs.</param>
    public TransferCommandHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUserRepository userRepository,
        IKycSubmissionRepository kycRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TransferDomainService transferDomainService,
        IMediator mediator,
        ILogger<TransferCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _kycRepository = kycRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transferDomainService = transferDomainService;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Handles a money transfer request between two accounts.
    /// Validates the request, checks authorization and account constraints,
    /// executes the transfer via the domain service, and persists all changes
    /// within a database transaction.
    /// </summary>
    /// <param name="request">The transfer command containing source, destination, and amount details.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="TransactionDto"/> representing the completed transfer transaction.
    /// </returns>
    /// <exception cref="UnauthorizedException">
    /// Thrown when the current user is not authenticated or has an invalid user identifier.
    /// </exception>
    /// <exception cref="ValidationException">
    /// Thrown when the transfer amount is invalid, accounts are the same,
    /// or currencies do not match.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when either the source or destination account cannot be found.
    /// </exception>
    /// <exception cref="ForbiddenAccessException">
    /// Thrown when the current user does not own the source account and is not an administrator.
    /// </exception>
    public async Task<TransactionDto> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        if (request.Amount <= 0)
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.Amount),
                    "Amount must be greater than zero.")
            });

        if (request.FromAccountId == request.ToAccountId)
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.FromAccountId),
                    "Cannot transfer to the same account.")
            });

        // if (!Guid.TryParse(_currentUserService.UserId, out var currentUserGuid))
        // throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

        var kyc = await _kycRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (kyc?.Status != KycStatus.Approved)
            throw new ForbiddenAccessException("KYC verification required to transfer.");

        var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId, cancellationToken);
        var toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId, cancellationToken);

        if (fromAccount == null)
            throw new NotFoundException(nameof(Account), request.FromAccountId);

        if (toAccount == null)
            throw new NotFoundException(nameof(Account), request.ToAccountId);

        if (fromAccount.UserId != user.Id && !_currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        if (fromAccount.Currency != toAccount.Currency)
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.CurrencyCode),
                    "Cross-currency transfers are not supported.")
            });

        var currency = CurrencyRegistry.FromCode(request.CurrencyCode);
        var money = new Money(request.Amount, currency);

        _logger.LogInformation(
            "Starting transfer {Amount} {Currency} from {From} to {To}",
            money.Amount,
            money.Currency.Code,
            fromAccount.Id,
            toAccount.Id);

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable,cancellationToken);

        try
        {
            var transaction = _transferDomainService.Transfer(
                fromAccount,
                toAccount,
                money,
                request.Description,
                new IdempotencyKey(request.IdempotencyKey)
            );

            _accountRepository.Update(fromAccount);
            _accountRepository.Update(toAccount);
            _transactionRepository.Add(transaction);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            foreach (var domainEvent in transaction.DomainEvents)
            {
                if (domainEvent is MoneyTransferredEvent moneyTransferred)
                    await _mediator.Publish(new MoneyTransferredNotification(moneyTransferred), cancellationToken);
            }
            transaction.ClearDomainEvents();

            _logger.LogInformation(
                "Transfer completed. TransactionId={TransactionId}",
                transaction.Id);

            Console.WriteLine(_unitOfWork.GetType().FullName);

            return _mapper.Map<TransactionDto>(transaction);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
