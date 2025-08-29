using Ardalis.Result;

using Contracts.Publishers.VoteService;

using VoteService.Domain.Entities;

namespace VoteService.Domain.Services;

public interface IAntiAbusePolicy {

  Task<Result> ValidateAsync (
    Guid actorUserId,
    Guid ownerUserId,
    Guid sourceId,
    VotableSourceType sourceType,
    VoteKind newKind,
    CancellationToken ct);

}