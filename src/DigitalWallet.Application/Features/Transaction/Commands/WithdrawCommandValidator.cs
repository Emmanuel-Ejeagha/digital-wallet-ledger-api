namespace DigitalWallet.Application.Features.Transaction.Commands;

public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator()
    {
        RuleFor(v => v.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required.");

        RuleFor(v => v.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.")
            .LessThanOrEqualTo(1_000_000_000);

        RuleFor(v => v.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3).WithMessage("Currency code must be 3 characters")
            .Matches("^[A_Z]{3}$")
            .WithMessage("Currency must be ISO code (USD, NGN, EUR)");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
