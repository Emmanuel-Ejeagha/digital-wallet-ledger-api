namespace DigitalWallet.Application.Features.Admin.Queries;

 public class GetAllUsersQuery : IRequest<List<UserDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        return _mapper.Map<List<UserDto>>(users);
    }
}
