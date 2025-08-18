using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.Publishers.ReputationService;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Repositories;

public interface IReputationRepository {

  Task<Result<ReputationEntry>> AppendEntryAsync (ReputationEntry entry, CancellationToken ct);
  Task<Result<IReadOnlyList<ReputationEntry>>> AppendEntriesAsync (IReadOnlyCollection<ReputationEntry> entries, CancellationToken ct);

  Task<Result<ReputationEntry>> GetEntryAsync (Guid id, CancellationToken ct);
  Task<Result<IReadOnlyList<ReputationEntry>>> GetEntriesAsync (Guid userId, CancellationToken ct);
  Task<Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>> GetPageEntriesAsync (
    Guid userId,
    PageParams pageParams,
    SortParams sortParams,
    CancellationToken ct);

  Task<Result> DeleteEntryAsync (Guid id, CancellationToken ct);
  Task<Result> DeleteEntriesAsync (Guid userId, CancellationToken ct);

  // Task<Result<ReputationSummary>> SetSummaryAsync (Guid userId, CancellationToken ct);
  // Task<Result<IReadOnlyList<ReputationSummary>>> SetSummaryAsync (IReadOnlyCollection<Guid> userIds, CancellationToken ct);
  
  Task SaveAsync (CancellationToken ct);

  Task<IReadOnlyList<UserReputationChanged>> ApplyVoteAsync(
    Guid eventId, string sourceService, string? correlationId,
    Guid entityId, ReputationSourceType st,
    Guid ownerUserId, int ownerNewAmount, ReasonCode ownerReason,
    DateTime occurredAt, int version, CancellationToken ct);
  
  Task<IReadOnlyList<UserReputationChanged>> ApplyAcceptedAnswerAsync(
    Guid eventId, string sourceService, string? correlationId,
    Guid questionId,
    Guid? oldOwnerUserId, int oldOwnerNewAmount,
    Guid? newOwnerUserId, int newOwnerNewAmount,
    DateTime occurredAt, int version, CancellationToken ct);

  Task<ReputationSummary?> GetSummaryAsync(Guid userId, CancellationToken ct);
  Task<IReadOnlyList<ReputationEffect>> GetEffectsAsync(Guid userId, CancellationToken ct);
}