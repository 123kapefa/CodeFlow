using Ardalis.Result;

using Contracts.Publishers.VoteService;

using Microsoft.Extensions.Caching.Distributed;

using VoteService.Domain.Entities;
using VoteService.Domain.Services;

namespace VoteService.Application.Services;

public sealed class DefaultAntiAbusePolicy : IAntiAbusePolicy {

  private readonly IDistributedCache _cache;
  public DefaultAntiAbusePolicy (IDistributedCache cache) => _cache = cache;

  public async Task<Result> ValidateAsync (
    Guid actorUserId,
    Guid ownerUserId,
    Guid entityId,
    VotableSourceType entityType,
    VoteKind newKind,
    CancellationToken ct) {
    if (actorUserId == ownerUserId)
      return Result.Conflict ("Нельзя проголосовать за свой собственный пост.");
    
    var flipKey = $"vote:flip:{actorUserId}:{entityType}:{entityId}";
    if (newKind != VoteKind.None && await _cache.GetStringAsync (flipKey, ct) is not null)
      return Result.Conflict ("Слишком много попыток. Попробуйте позже.");
    await _cache.SetStringAsync (flipKey, "1",
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes (5) }, ct);
    
    var capKey = $"vote:cap:{actorUserId}:{ownerUserId}:{DateOnly.FromDateTime (DateTime.UtcNow)}";
    var bytes = await _cache.GetAsync (capKey, ct);
    var count = bytes is null ? 0 : BitConverter.ToInt32 (bytes);
    count++;
    if (count > 20) return Result.Conflict ("Превышено ежедневное ограничение этого пользователя.");
    await _cache.SetAsync (capKey, BitConverter.GetBytes (count),
      new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays (1) }, ct);
    
    return Result.Success ();
  }

}