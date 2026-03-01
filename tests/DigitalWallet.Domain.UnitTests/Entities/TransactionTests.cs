namespace Domain.UnitTests.Entities;

public class TransactionTests
{
    private readonly Account _fromAccount;
    private readonly Account _toAccount;
    private readonly Money _amount;
    private readonly IdempotencyKey _idempotencyKey;

    public TransactionTests()
    {
        var userId = Guid.NewGuid();
        _fromAccount = new Account(userId, AccountType.Personal, Currency.USD, "Sender");
        _toAccount = new Account(userId, AccountType.Personal, Currency.USD, "Receiver");
        _amount = new Money(100, Currency.USD);
        _idempotencyKey = new IdempotencyKey("test-key");
    }

    [Fact]
    public void Constructor_Should_Create_Pending_Transaction()
    {
        // Act
        var transaction = new Transaction("REF123", "Test transfer", _idempotencyKey);

        // Assert
        transaction.Reference.Should().Be("REF123");
        transaction.Description.Should().Be("Test transfer");
        transaction.IdempotencyKey.Should().Be(_idempotencyKey);
        transaction.Status.Should().Be(TransactionStatus.Pending);
        transaction.CompletedAt.Should().BeNull();
        transaction.Entries.Should().BeEmpty();
    }

    [Fact]
    public void AddEntry_Should_Add_To_Collection()
    {
        // Arrange
        var transaction = new Transaction("REF123", "Test", _idempotencyKey);
        var entry = new LedgerEntry(_fromAccount.Id, transaction.Id, EntryType.Debit, _amount, 100, "Test");

        // Act
        transaction.AddEntry(entry);

        // Assert
        transaction.Entries.Should().HaveCount(1);
        transaction.Entries.First().Should().Be(entry);
    }

    [Fact]
    public void AddEntry_After_Completion_Should_Throw()
    {
        // Arrange
        var transaction = new Transaction("REF123", "Test", _idempotencyKey);
        var debit = new LedgerEntry(_fromAccount.Id, transaction.Id, EntryType.Debit, _amount, 100, "");
        var credit = new LedgerEntry(_toAccount.Id, transaction.Id, EntryType.Credit, _amount, 0, "");
        transaction.AddEntry(debit);
        transaction.AddEntry(credit);
        transaction.Complete();

        var extraEntry = new LedgerEntry(_fromAccount.Id, transaction.Id, EntryType.Debit, _amount, 200, "");

        // Act
        var act = () => transaction.AddEntry(extraEntry);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add entry to a non‑pending transaction.");
    }

    [Fact]
    public void Complete_With_Unbalanced_Entries_Should_Throw()
    {
        // Arrange
        var transaction = new Transaction("REF123", "Test", _idempotencyKey);
        var debit = new LedgerEntry(_fromAccount.Id, transaction.Id, EntryType.Debit, _amount, 100, "");
        transaction.AddEntry(debit); // only debit, no credit

        // Act
        var act = () => transaction.Complete();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Double‑entry invariant violated*");
    }

    [Fact]
    public void Complete_With_Balanced_Entries_Should_Set_Status_To_Completed()
    {
        // Arrange
        var transaction = new Transaction("REF123", "Test", _idempotencyKey);
        var debit = new LedgerEntry(_toAccount.Id, transaction.Id, EntryType.Debit, _amount, 100, "");
        var credit = new LedgerEntry(_fromAccount.Id, transaction.Id, EntryType.Credit, _amount, 0, "");
        transaction.AddEntry(debit);
        transaction.AddEntry(credit);

        // Act
        transaction.Complete();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.CompletedAt.Should().NotBeNull();
    }
}