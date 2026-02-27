namespace DigitalWallet.Application.Common.Attributes;
/// <summary>
/// Attribute used to mark a MediatR request as idempotent.
/// When applied, the IdempotencyBehavior will ensure the request
/// is processed only once
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IdempotentAttribute : Attribute
{
    /// <summary>
    /// Indicates where the idempotency key originates (e.g., Header, Body)
    /// Reserved for future use
    /// </summary>
    public string KeySource { get; }
    public IdempotentAttribute(string keySource = "Header")
    {
        KeySource = keySource;
    }

}