using AutoMapper;
using DigitalWallet.Application.Common.Behaviors;
using DigitalWallet.Application.Common.Exceptions;
using DigitalWallet.Application.Common.Interfaces;
using DigitalWallet.Application.DTOs;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Exceptions;
using DigitalWallet.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.Features.Accounts.Commands;
/// <summary>
/// Command to create a new wallet account for the current user
/// </summary>
[Idempotent]
public class CreateAccountCommand : IRequest<AccountDto>
{
    public string IdempotencyKey { get; set; } = string.Empty; // for idempotency attribute
    public string CurrencyCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Personal";
    public string Name { get; set; } = string.Empty;
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ILogger<CreateAccountCommandHandler> _logger;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateAccountCommandHandler> logger
    )
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var user = await _userRepository.GetByAuth0IdAsync(_currentUserService.UserId!, cancellationToken);

        if (user == null)
            throw new NotFoundException(nameof(User), _currentUserService.UserId!);

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
