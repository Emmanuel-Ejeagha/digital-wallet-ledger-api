namespace DigitalWallet.Application.Features.Accounts.Queries;

public class GetUserAccountQuery : IRequest<List<AccountDto>>
{

}

public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountQuery, List<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserAccountsQueryHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(GetUserAccountQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();
        
        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

        var accounts = await _accountRepository.GetByUserIdAsync(user.Id, cancellationToken);
        return _mapper.Map<List<AccountDto>>(accounts);
    }
}
