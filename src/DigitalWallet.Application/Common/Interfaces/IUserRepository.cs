using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Repository for User entity.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByAuth0IdAsync(string auth0UserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Email must already be normalized (lowercase + trimmed)
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    void Add(User user);
    void Update(User user);
}
