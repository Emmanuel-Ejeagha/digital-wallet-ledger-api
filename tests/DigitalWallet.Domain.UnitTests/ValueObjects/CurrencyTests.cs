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
        usd.Name.Should().Be("US Dollar");

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
        var currency2 = Currency.Create("USD", 2, "$", "US Dollar");
        var currency3 = Currency.EUR;

        // Assert
        currency1.Should().Be(currency2); // same code
        currency1.Should().NotBe(currency3);
    }
}
