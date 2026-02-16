using DigitalWallet.Domain.Base;

namespace DigitalWallet.Domain.ValueObjects;
/// <summary>
/// Idempotency key value object. Used to ensure that a request is processed only once
/// </summary>
public sealed class IdempotencyKey : ValueObject
{
    public string Value { get; }

    public IdempotencyKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Idempotency key cannot be empty.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
