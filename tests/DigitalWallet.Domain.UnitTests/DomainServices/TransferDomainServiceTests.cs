namespace Domain.UnitTests.DomainServices;

public class TransferDomainServiceTests
{
    private readonly TransferDomainService _service = new();
    private readonly Account _fromAccount;
    private readonly Account _toAccount;
    private readonly Money _amount;
    private readonly IdempotencyKey _idempotencyKey;

    public TransferDomainServiceTests()
    {
        var userId = Guid.NewGuid();
        _fromAccount = new Account(userId, AccountType.Personal, Currency.USD, "Sender");
        _toAccount = new Account(userId, AccountType.Personal, Currency.USD, "Receiver");
        _fromAccount.ApplyDebit(new Money(500, Currency.USD)); // initial balance 500
        _amount = new Money(100, Currency.USD);
        _idempotencyKey = new IdempotencyKey("transfer-key");
    }

    [Fact]
    public void Transfer_Should_Create_Transaction_With_Debit_And_Credit_Entries()
    {
        // Act
        var transaction = _service.Transfer(_fromAccount, _toAccount, _amount, "Test transfer", _idempotencyKey);

        // Assert
        transaction.Should().NotBeNull();
        transaction.Reference.Should().StartWith("TXN-");
        transaction.Description.Should().Be("Test transfer");
        transaction.IdempotencyKey.Should().Be(_idempotencyKey);
        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.Entries.Should().HaveCount(2);

        var credit = transaction.Entries.Single(e => e.Type == EntryType.Credit);
        var debit = transaction.Entries.Single(e => e.Type == EntryType.Debit);

        credit.AccountId.Should().Be(_fromAccount.Id);
        credit.Amount.Should().Be(_amount);
        credit.BalanceAfter.Should().Be(400); // 500 - 100

        debit.AccountId.Should().Be(_toAccount.Id);
        debit.Amount.Should().Be(_amount);
        debit.BalanceAfter.Should().Be(100); // 0 + 100

        _fromAccount.Balance.Should().Be(400);
        _toAccount.Balance.Should().Be(100);
    }

    [Fact]
    public void Transfer_With_Insufficient_Balance_Should_Throw()
    {
        // Arrange
        var largeAmount = new Money(1000, Currency.USD);

        // Act
        var act = () => _service.Transfer(_fromAccount, _toAccount, largeAmount, "Test", _idempotencyKey);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient balance for credit.");
    }

    [Fact]
    public void Transfer_With_Different_Currencies_Should_Throw()
    {
        // Arrange
        var eurAmount = new Money(100, Currency.EUR);

        // Act
        var act = () => _service.Transfer(_fromAccount, _toAccount, eurAmount, "Test", _idempotencyKey);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*does not match transfer currency*");
    }

    [Fact]
    public void Transfer_When_Source_Account_Inactive_Should_Throw()
    {
        // Arrange
        _fromAccount.Deactivate();

        // Act
        var act = () => _service.Transfer(_fromAccount, _toAccount, _amount, "Test", _idempotencyKey);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Source account is not active.");
    }

    [Fact]
    public void Transfer_Should_Raise_MoneyTransferredEvent()
    {
        // Act
        var transaction = _service.Transfer(_fromAccount, _toAccount, _amount, "Test", _idempotencyKey);

        // Assert
        transaction.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "MoneyTransferredEvent");
    }
}