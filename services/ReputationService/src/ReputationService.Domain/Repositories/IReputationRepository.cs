using Ardalis.Result;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Repositories;

public sealed record ChangedUserReputation (Guid UserId, int OldTotal, int NewTotal, int Delta);

public interface IReputationRepository {

  Task<Result<ReputationEntry>> AppendEntryAsync (ReputationEntry entry, CancellationToken ct);

  Task<Result<IReadOnlyList<ChangedUserReputation>>> AppendEntriesAndUpdateSnapshotsAsync (
    IReadOnlyCollection<ReputationEntry> entries,
    CancellationToken ct);

  Task<Result<ReputationEntry>> GetEntryAsync (Guid id, CancellationToken ct);

  Task<Result<IReadOnlyList<ReputationEntry>>> GetEntriesAsync (Guid userId, CancellationToken ct);

  Task<Result> DeleteEntryAsync (Guid id, CancellationToken ct);

  Task<Result> DeleteEntriesAsync (Guid userId, CancellationToken ct);

}