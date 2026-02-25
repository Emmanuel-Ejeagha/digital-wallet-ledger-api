namespace DigitalWallet.Application.Features.Transaction.Commands;
/// <summary>Validates TransferCommand</summary>
public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(v => v.IdempotencyKey)
            .Must(k => !string.IsNullOrWhiteSpace(k))
            .WithMessage("Idempotencykey is required.");

        RuleFor(v => v.FromAccountId)
            .NotEqual(Guid.Empty).WithMessage("Source account ID is required")
            .NotEqual(v => v.ToAccountId)
            .WithMessage("Source and destination accounts must be different.");

        RuleFor(v => v.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(v => v.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3).WithMessage("Currency code must be 3 characters.")
            .Matches("^[A-Z{3}]$")
            .WithMessage("Currency code must be 3 uppercase letters.");

        RuleFor(v => v.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}
