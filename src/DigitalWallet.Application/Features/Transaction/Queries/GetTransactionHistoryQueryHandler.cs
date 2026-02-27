namespace DigitalWallet.Application.Features.Transaction.Queries;
/// <summary>
/// Handles retrival of account transaction history with authorization checks.
/// </summary>
public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, List<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTransactionHistoryQueryHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken) ?? throw new NotFoundException(nameof(Account), request.AccountId);

        if (account.UserId.ToString() != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        if (request.Page <= 0)
            request.Page = 1;

        if (request.PageSize <= 0 || request.PageSize > 200)
            request.PageSize = 20;

        var transactions = await _transactionRepository.GetByAccountIdAsync(
            request.AccountId,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            cancellationToken);

        return _mapper.Map<List<TransactionDto>>(transactions);
    }
}
