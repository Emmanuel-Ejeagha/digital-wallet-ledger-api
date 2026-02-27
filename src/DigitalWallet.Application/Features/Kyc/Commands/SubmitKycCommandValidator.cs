namespace DigitalWallet.Application.Features.Kyc.Commands;

public class SubmitKycCommandValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycCommandValidator()
    {
        RuleFor(v => v.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required.");

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 chacters");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(v => v.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("You must be at least 18 years old.");

        RuleFor(v => v.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required.")
            .MaximumLength(200).WithMessage("Address line 1 must not exceed 200 characters.");

        RuleFor(v => v.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(v => v.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(v => v.DocumentType)
            .NotEmpty().WithMessage("Document type is required.")
            .MaximumLength(50).WithMessage("Document type must not exceed 50 characters.");

        RuleFor(v => v.DocumentNumber)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document type must not exceed 50 characters.");

        RuleFor(v => v.DocumentFile)
            .NotEmpty().WithMessage("Document file is required")
            .Must(file => file.Length > 0).WithMessage("Document file cannot be empty")
            .Must(file => file.Length <= 10 * 1024 * 1024).WithMessage("Document file must not exceed 10 MB")
            .Must(file => IsValidFileType(file)).WithMessage("Only PDF, JPG, or PHG files are allowed");
    }

    private bool IsValidFileType(IFormFile file)
    {
        var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
        return allowedTypes.Contains(file.ContentType);
    }
}
