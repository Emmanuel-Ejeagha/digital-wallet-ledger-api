namespace DigitalWallet.Application.Common.Interfaces;
/// <summary>
/// Provides information about the current user making request.
/// Implemented in the API layer (using HttpContext)
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
