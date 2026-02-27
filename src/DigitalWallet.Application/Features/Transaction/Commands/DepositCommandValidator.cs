namespace DigitalWallet.Application.Features.Transaction.Commands;
/// <summary>
/// Validates DepositCommand
/// </summary>
public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(v => v.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required.");

        RuleFor(v => v.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

    RuleFor(v => v.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required")
            .Length(3).WithMessage("Currency code must be 3 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
