namespace DigitalWallet.Infrastructure.Repositories;

public class KycSubmissionRepository : IKycSubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public KycSubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KycSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KycSubmissions
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<KycSubmission?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.KycSubmissions
            .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);
    }

    public async Task<List<KycSubmission>> GetAllAsync(KycStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.KycSubmissions.AsQueryable();
        if (status.HasValue)
            query = query.Where(k => k.Status == status.Value);
        return await query
            .OrderByDescending(k => k.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public void Add(KycSubmission submission)
    {
        _context.KycSubmissions.Add(submission);
    }

    public void Update(KycSubmission submission)
    {
        _context.Entry(submission).State = EntityState.Modified;
    }

    public void Remove(KycSubmission submission)
    {
        _context.KycSubmissions.Remove(submission);
    }
}
