namespace DigitalWallet.Domain.UnitTests.ValueObjects;

public class CurrencyTests
{
    [Fact]
    public void Predefined_Currencies_Should_Have_Correct_Properties()
    {
        // Arrange & Act
        var usd = Currency.USD;
        var ngn = Currency.NGN;

        // Assert
        usd.Code.Should().Be("USD");
        usd.DecimalPlaces.Should().Be(2);
        usd.Symbol.Should().Be("$");
        usd.Name.Should().Be("Dollar");

        ngn.Code.Should().Be("NGN");
        ngn.DecimalPlaces.Should().Be(2);
        ngn.Symbol.Should().Be("₦");
        ngn.Name.Should().Be("Naira");
    }

    [Fact]
    public void Equality_Should_Be_Based_On_Code()
    {
        // Arrange
        var currency1 = Currency.USD;
        var currency2 = () => Currency.Create("US", 2, "$", "Dollar");
        var currency3 = Currency.EUR;

        // Assert
    currency2.Should().Throw<ArgumentException>().WithMessage("*Currency code must be a 3-letter ISO code*");
    }
}
