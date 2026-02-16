using DigitalWallet.Domain.Base;

namespace DigitalWallet.Domain.Entities;
/// <summary>
/// Application user. Identity is managed by Auth0; we store the Auth user ID.
/// </summary>
public class User : AuditableEntity
{
    public string Auth0UserId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private User() { } // EF Core

    public User(string auth0UserId, string email, string firstName, string lastname) : base()
    {
        if (string.IsNullOrWhiteSpace(auth0UserId))
            throw new ArgumentException("Auth0 user ID cannot be empty.", nameof(auth0UserId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First Name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastname))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastname));

        Auth0UserId = auth0UserId.Trim();
        Email = email.Trim().ToLowerInvariant();
        FirstName = firstName.Trim();
        LastName = lastname.Trim();
        IsActive = true;
    }

    public void UpdateProfile(string firstName, string lastname)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastname))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastname));
        
        FirstName = firstName.Trim();
        LastName = lastname.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
