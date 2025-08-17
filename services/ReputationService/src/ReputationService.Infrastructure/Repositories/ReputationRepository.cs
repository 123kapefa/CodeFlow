using Ardalis.Result;

using Microsoft.EntityFrameworkCore;

using ReputationService.Domain.Entities;
using ReputationService.Domain.Repositories;

namespace ReputationService.Infrastructure.Repositories;

public sealed class ReputationRepository : IReputationRepository
{
    private readonly ReputationServiceDbContext _context;

    public ReputationRepository(ReputationServiceDbContext context) => _context = context;

    public Task<Result<ReputationEntry>> AppendEntryAsync (ReputationEntry entry, CancellationToken ct) {
        throw new NotImplementedException ();
    }
    public Task<Result<IReadOnlyList<ChangedUserReputation>>> AppendEntriesAndUpdateSnapshotsAsync (IReadOnlyCollection<ReputationEntry> entries, CancellationToken ct) {
        throw new NotImplementedException ();
    }
    public Task<Result<ReputationEntry>> GetEntryAsync (Guid id, CancellationToken ct) {
        throw new NotImplementedException ();
    }
    public Task<Result<IReadOnlyList<ReputationEntry>>> GetEntriesAsync (Guid userId, CancellationToken ct) {
        throw new NotImplementedException ();
    }
    public Task<Result> DeleteEntryAsync (Guid id, CancellationToken ct) {
        throw new NotImplementedException ();
    }
    public Task<Result> DeleteEntriesAsync (Guid userId, CancellationToken ct) {
        throw new NotImplementedException ();
    }

}