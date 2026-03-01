namespace Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Amount_Negative()
    {
        // Act
        var act = () => new Money(-10, Currency.USD);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Money amount cannot be negative.");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Amount_Has_Too_Many_Decimals()
    {
        // Act
        var act = () => new Money(10.123m, Currency.USD); // USD allows 2 decimals

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*more decimal places*");
    }

    [Fact]
    public void Constructor_Should_Allow_Valid_Amount()
    {
        // Act
        var money = new Money(10.50m, Currency.USD);

        // Assert
        money.Amount.Should().Be(10.50m);
        money.Currency.Should().Be(Currency.USD);
    }

    [Fact]
    public void FromSmallestUnit_Should_Create_Correct_Amount()
    {
        // Act
        var money = Money.FromSmallestUnit(1000, Currency.USD);

        // Assert
        money.Amount.Should().Be(10.00m);
        money.Currency.Should().Be(Currency.USD);
    }

    [Fact]
    public void Addition_Same_Currency_Should_Succeed()
    {
        // Arrange
        var m1 = new Money(10, Currency.USD);
        var m2 = new Money(5, Currency.USD);

        // Act
        var sum = m1 + m2;

        // Assert
        sum.Amount.Should().Be(15);
        sum.Currency.Should().Be(Currency.USD);
    }

    [Fact]
    public void Addition_Different_Currency_Should_Throw()
    {
        // Arrange
        var m1 = new Money(10, Currency.USD);
        var m2 = new Money(5, Currency.EUR);

        // Act
        var act = () => m1 + m2;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies.");
    }

    [Fact]
    public void Subtraction_Same_Currency_With_Sufficient_Funds_Should_Succeed()
    {
        // Arrange
        var m1 = new Money(10, Currency.USD);
        var m2 = new Money(5, Currency.USD);

        // Act
        var result = m1 - m2;

        // Assert
        result.Amount.Should().Be(5);
    }

    [Fact]
    public void Subtraction_Insufficient_Funds_Should_Throw()
    {
        // Arrange
        var m1 = new Money(5, Currency.USD);
        var m2 = new Money(10, Currency.USD);

        // Act
        var act = () => m1 - m2;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient funds for subtraction.");
    }

    [Fact]
    public void Round_Should_Apply_Bankers_Rounding()
    {
        // Arrange
        var money = new Money(1.2345m, Currency.USD);

        // Act
        var rounded = money.Round();

        // Assert
        rounded.Amount.Should().Be(1.23m);
    }
}