using Ardalis.Result;

using Contracts.Publishers.VoteService;

using VoteService.Domain.Entities;

namespace VoteService.Domain.Repositories;

public interface IVoteRepository {

  Task<Result<Vote>> GetAsync (Guid actorUserId, VotableSourceType sourceType, Guid sourceId, CancellationToken ct);
  Task<Vote> AddAsync (Vote vote, CancellationToken ct);
  Task UpdateAsync (Vote vote, CancellationToken ct);
  Task SaveChangesAsync (CancellationToken ct);
}