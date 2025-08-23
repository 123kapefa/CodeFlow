using Ardalis.Result;

using Contracts.Publishers.VoteService;

using Microsoft.EntityFrameworkCore;

using VoteService.Domain.Entities;
using VoteService.Domain.Repositories;

namespace VoteService.Infrastructure.Repositories;

public sealed class VoteRepository : IVoteRepository
{
  private readonly VoteServiceDbContext _db;
  public VoteRepository(VoteServiceDbContext db) => _db = db;

  public async Task<Result<Vote>> GetAsync (Guid actorUserId, VotableSourceType type, Guid entityId, CancellationToken ct) {
    var vote = await _db.Votes.FirstOrDefaultAsync (v => v.AuthorUserId == actorUserId && v.SourceType == type && v.SourceId == entityId,
      ct);
    
    if (vote is null) return Result<Vote>.NotFound();
    
    return vote;
  }

  public async Task<Vote> AddAsync(Vote vote, CancellationToken ct)
  {
    _db.Votes.Add(vote);
    await _db.SaveChangesAsync(ct);
    return vote;
  }

  public Task UpdateAsync(Vote vote, CancellationToken ct)
    => _db.SaveChangesAsync(ct);
  
  public async Task SaveChangesAsync(CancellationToken ct) =>
    await _db.SaveChangesAsync(ct);
}
