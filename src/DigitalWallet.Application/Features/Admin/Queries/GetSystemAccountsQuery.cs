namespace DigitalWallet.Application.Features.Admin.Queries;

public class GetSystemAccountsQuery : IRequest<List<AccountDto>> { }

public class GetSystemAccountsQueryHandler : IRequestHandler<GetSystemAccountsQuery, List<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetSystemAccountsQueryHandler(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(GetSystemAccountsQuery request, CancellationToken cancellationToken)
    {
        var systemUser = await _userRepository.GetByEmailAsync("system@internal", cancellationToken);
        if (systemUser == null)
            return new List<AccountDto>();

        var accounts = await _accountRepository.GetByUserIdAsync(systemUser.Id, cancellationToken);
        return _mapper.Map<List<AccountDto>>(accounts);
    }
}
