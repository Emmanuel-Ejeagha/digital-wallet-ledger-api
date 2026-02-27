namespace DigitalWallet.Application.Common.Interfaces.Repositories;
/// <summary>
/// Reopository interface for KycSubmission
/// </summary>
public interface IKycSubmissionRepository
{
    Task<KycSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<KycSubmission?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<KycSubmission>> GetAllAsync(KycStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    void Add(KycSubmission submission);
    void Update(KycSubmission submission);
    void Remove(KycSubmission submission);    
}
