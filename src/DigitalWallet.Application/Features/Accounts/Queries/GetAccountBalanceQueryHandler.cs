namespace DigitalWallet.Application.Features.Accounts.Queries;

public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetAccountBalanceQueryHandler()
    {
    }

    public GetAccountBalanceQueryHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();
        
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
            throw new NotFoundException(nameof(Account), request.AccountId);
            
        if (account.UserId.ToString() != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        return _mapper.Map<AccountDto>(account);
    }
}

