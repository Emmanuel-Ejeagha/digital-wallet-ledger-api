namespace DigitalWallet.Domain.Base;
/// <summary>
/// Base class for entitties that require audit information (created/updated timestamps and users)
/// </summary>
public abstract class AuditableEntity : Entity
{
    protected AuditableEntity() { }
    protected AuditableEntity(Guid id) : base(id) { }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } // Auth) user identifier
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
