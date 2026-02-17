using System;
using AutoMapper;
using DigitalWallet.Application.Common.Interfaces;
using DigitalWallet.Application.DTOs;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.Features.Users.Commands;
/// <summary>
/// Command to register a new user (after Auth0 signup
/// </summary>
public class RegisterUserCommand : IRequest<UserDto>
{
    public string Auth0UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByAuth0IdAsync(request.Auth0UserId, cancellationToken))
        {
            _logger.LogInformation("Attempted to register an existing user: {AuthUserId}", request.Auth0UserId);
            throw new DomainException("User already exists.");
        }

        var user = new User(
            request.Auth0UserId,
            request.Email,
            request.FirstName,
            request.LastName);
        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} created successfully with Id {UserId}",
            user.Email, user.Id);

        return _mapper.Map<UserDto>(user);
    }
}
