using DigitalWallet.Application.Features.Accounts.Commands;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.UnitTests.Validators;

public class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_With_Valid_Command_Should_Not_Have_Errors()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            IdempotencyKey = "key123",
            CurrencyCode = "USD",
            Name = "My Wallet",
            AccountType = "Personal"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_With_Missing_IdempotencyKey_Should_Have_Error()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            IdempotencyKey = "",
            CurrencyCode = "USD",
            Name = "Wallet",
            AccountType = "Personal"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdempotencyKey)
            .WithErrorMessage("Idempotency key is required.");
    }

    [Fact]
    public void Validate_With_Invalid_CurrencyCode_Should_Have_Error()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            IdempotencyKey = "key123",
            CurrencyCode = "US", // too short
            Name = "Wallet",
            AccountType = "Personal"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrencyCode)
            .WithErrorMessage("Currency code must be 3 characters.");
    }

    [Fact]
    public void Validate_With_Missing_Name_Should_Have_Error()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            IdempotencyKey = "key123",
            CurrencyCode = "USD",
            Name = "",
            AccountType = "Personal"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Account name is required.");
    }
}