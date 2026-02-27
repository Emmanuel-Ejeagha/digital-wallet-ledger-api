namespace DigitalWallet.Application.Common.Interfaces.Repositories;
/// <summary>Repository for account aggregate.</summary>
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Account?> GetByUserAndCurrencyAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default);    
    void Add(Account account);
    void Update(Account account);
    void Remove(Account account);
}
