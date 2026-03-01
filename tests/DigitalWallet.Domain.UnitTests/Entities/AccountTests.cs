namespace Domain.UnitTests.Entities;

public class AccountTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Currency _usd = Currency.USD;

    [Fact]
    public void Constructor_Should_Create_Account_With_Zero_Balance()
    {
        // Act
        var account = new Account(_userId, AccountType.Personal, _usd, "My Wallet");

        // Assert
        account.UserId.Should().Be(_userId);
        account.Type.Should().Be(AccountType.Personal);
        account.Currency.Should().Be(_usd);
        account.Name.Should().Be("My Wallet");
        account.Balance.Should().Be(0);
        account.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ApplyDebit_Should_Increase_Balance()
    {
        // Arrange
        var account = new Account(_userId, AccountType.Personal, _usd, "My Wallet");
        var money = new Money(100, _usd);

        // Act
        account.ApplyDebit(money);

        // Assert
        account.Balance.Should().Be(100);
    }

    [Fact]
    public void ApplyCredit_Should_Decrease_Balance_When_Sufficient()
    {
        // Arrange
        var account = new Account(_userId, AccountType.Personal, _usd, "My Wallet");
        account.ApplyDebit(new Money(200, _usd)); // balance = 200

        // Act
        account.ApplyCredit(new Money(150, _usd));

        // Assert
        account.Balance.Should().Be(50);
    }

    [Fact]
    public void ApplyCredit_With_Insufficient_Balance_Should_Throw()
    {
        // Arrange
        var account = new Account(_userId, AccountType.Personal, _usd, "My Wallet");
        account.ApplyDebit(new Money(50, _usd));

        // Act
        var act = () => account.ApplyCredit(new Money(100, _usd));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient balance for credit.");
    }

    [Fact]
    public void ApplyDebit_When_Account_Inactive_Should_Throw()
    {
        // Arrange
        var account = new Account(_userId, AccountType.Personal, _usd, "My Wallet");
        account.Deactivate();

        // Act
        var act = () => account.ApplyDebit(new Money(100, _usd));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Account is not active.");
    }
}