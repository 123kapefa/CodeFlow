using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.DTOs.ReputationService;
using Contracts.Publishers.ReputationService;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Repositories;

public interface IReputationRepository {

  Task SaveAsync (CancellationToken ct);

  Task<IReadOnlyList<UserReputationChanged>> ApplyVoteAsync (
    Guid sourceEventId,
    Guid parentId,
    Guid sourceId,
    Guid ownerUserId,
    string sourceService,
    ReputationSourceType sourceType,
    int delta,
    ReasonCode ownerReason,
    DateTime occurredAt,
    int version,
    CancellationToken ct);

  Task<IReadOnlyList<UserReputationChanged>> ApplyAcceptedAnswerAsync (
    Guid eventId,
    Guid parentId,
    string sourceService,
    Guid? oldOwnerUserId,
    int oldDelta,
    Guid newAnswerId,
    Guid newOwnerUserId,
    int newDelta,
    ReasonCode reason,
    DateTime occurredAt,
    CancellationToken ct);

  Task<ReputationSummary?> GetSummaryAsync (Guid userId, CancellationToken ct);

  Task<Result<IReadOnlyList<IGrouping<DateTime, ReputationEntry>>>> GetMonthReputationAsync (
    Guid userId,
    CancellationToken ct);

  Task<Result<(IReadOnlyList<IGrouping<DateTime, ReputationEntry>> Items, PagedInfo Page)>> GetReputationShortList (
    Guid userId,
    PageParams pageParams,
    CancellationToken ct);

  Task<Result<(IReadOnlyList<IGrouping<DateOnly, ReputationEntry>> Groups, PagedInfo Page)>> GetReputationFullList (
    Guid userId,
    PageParams pageParams,
    CancellationToken ct);

}