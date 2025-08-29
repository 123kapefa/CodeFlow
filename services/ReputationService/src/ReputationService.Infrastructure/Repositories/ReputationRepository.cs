using System.Text.Json;

using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.DTOs.ReputationService;
using Contracts.Publishers.ReputationService;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReputationService.Domain.Entities;
using ReputationService.Domain.Repositories;
using ReputationService.Infrastructure.Extensions;

namespace ReputationService.Infrastructure.Repositories;

public sealed class ReputationRepository : IReputationRepository {

  private readonly ReputationServiceDbContext _context;
  private readonly ILogger<ReputationRepository> _logger;

  public ReputationRepository (ReputationServiceDbContext context, ILogger<ReputationRepository> logger) {
    _context = context;
    _logger = logger;
  }

  public async Task<Result> DeleteEntriesAsync (Guid userId, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.DeleteEntryAsync: Удаление записей об изменении репутации пользователя.");

      //var reputationEntries = await GetEntriesAsync (userId, ct);

      //_context.ReputationEntries.RemoveRange ((List<ReputationEntry>)reputationEntries);

      _logger.LogInformation ($"ReputationRepository.DeleteEntryAsync: Записи успешно удалены.");
      return Result.Success ();
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.DeleteEntryAsync: отмена операции.");
      return Result.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.DeleteEntryAsync: База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }

  public async Task SaveAsync (CancellationToken ct) => await _context.SaveChangesAsync (ct);

  public async Task<IReadOnlyList<UserReputationChanged>> ApplyVoteAsync (
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
    CancellationToken ct) {
    var changes = new List<UserReputationChanged> (1);
    await using var tx = await _context.Database.BeginTransactionAsync (ct);

    // единственный эффект: владелец поста
    var deltaResult = await UpsertEffectDelta (ownerUserId, parentId, sourceId, sourceType, "VoteOwner", delta, version,
      sourceEventId, sourceService, ct);

    if (deltaResult != 0) {
      // (опц.) ledger
      _context.ReputationEntries.Add (ReputationEntry.Create (ownerUserId, parentId, sourceId, sourceType, "VoteOwner",
        ownerReason, deltaResult, occurredAt, sourceEventId, sourceService));
      await _context.SaveChangesAsync (ct);

      // summary
      changes.Add (await ApplyDeltaToSummary (ownerUserId, deltaResult, ct));
      await _context.SaveChangesAsync (ct);
    }

    await tx.CommitAsync (ct);
    return changes;
  }

  public async Task<IReadOnlyList<UserReputationChanged>> ApplyAcceptedAnswerAsync (
    Guid eventId,
    Guid parentId,
    string sourceService,
    Guid? oldAnswerId,  
    Guid? oldOwnerUserId,
    int oldDelta,
    Guid newAnswerId,
    Guid newOwnerUserId,
    int newDelta,
    ReasonCode reason,
    DateTime occurredAt,
    CancellationToken ct) {
    var changes = new List<UserReputationChanged> (2);
    await using var tx = await _context.Database.BeginTransactionAsync (ct);

    if (oldOwnerUserId is not null && oldAnswerId is not null) {
      var eff = await GetOrCreateEffect ((Guid)oldOwnerUserId!, parentId, (Guid)oldAnswerId!, ReputationSourceType.Answer, "AcceptedAnswer",
        sourceService, ct);
      if (oldDelta != 0) {
        var applied = eff.Apply (oldDelta, eventId);
        if (applied) {
          _context.ReputationEntries.Add (ReputationEntry.Create ((Guid)oldOwnerUserId, parentId, (Guid)oldAnswerId, ReputationSourceType.Answer,
            "AcceptedAnswer", reason, oldDelta, occurredAt, eventId, sourceService));
          changes.Add (await ApplyDeltaToSummary ((Guid)oldOwnerUserId, oldDelta, ct));
        }
      }
    }

    if (newOwnerUserId is Guid newId) {
      var eff = await GetOrCreateEffect (newId, parentId, newAnswerId, ReputationSourceType.Answer, "AcceptedAnswer",
        sourceService, ct);
      if (newDelta != 0) {
        var applied = eff.Apply (newDelta, eventId);
        if (applied) {
          _context.ReputationEntries.Add (ReputationEntry.Create (newId, parentId, newAnswerId,
            ReputationSourceType.Answer, "AcceptedAnswer", reason, newDelta, occurredAt, eventId, sourceService));
          changes.Add (await ApplyDeltaToSummary (newId, newDelta, ct));
        }
      }
    }

    await _context.SaveChangesAsync (ct);
    await tx.CommitAsync (ct);      

        return changes.ToList ();
  }

  public Task<ReputationSummary?> GetSummaryAsync (Guid userId, CancellationToken ct) =>
    _context.ReputationSummaries.AsNoTracking ().FirstOrDefaultAsync (x => x.UserId == userId, ct);

  public async Task<Result<(IReadOnlyList<IGrouping<DateOnly, ReputationEntry>> Groups, PagedInfo Page)>>
    GetReputationFullList (Guid userId, PageParams pageParams, CancellationToken ct) {
    var entries = await _context.ReputationEntries.Where (x => x.UserId == userId).ToListAsync (ct);

    var grouped = entries.GroupBy (x => DateOnly.FromDateTime (x.OccurredAt.Date)).OrderByDescending (g => g.Key)
     .ToList ();

    var totalRecords = grouped.Count;

    var paged = grouped.Skip ((pageParams.Page!.Value - 1) * pageParams.PageSize!.Value)
     .Take (pageParams.PageSize.Value).ToList ();

    var pageInfo = new PagedInfo (pageNumber: pageParams.Page.Value, pageSize: pageParams.PageSize.Value,
      totalPages: (int)Math.Ceiling (totalRecords / (double)pageParams.PageSize.Value), totalRecords: totalRecords);

    return Result<(IReadOnlyList<IGrouping<DateOnly, ReputationEntry>> Groups, PagedInfo Page)>
     .Success ((Groups: paged, Page: pageInfo));
  }

  public async Task<Result<(IReadOnlyList<IGrouping<DateTime, ReputationEntry>> Items, PagedInfo Page)>>
    GetReputationShortList (Guid userId, PageParams pageParams, CancellationToken ct) {
    var fromDate = DateTime.UtcNow.Date.AddDays (-30);

    var entries = await _context.ReputationEntries.Where (x => x.UserId == userId && x.OccurredAt >= fromDate)
     .ToListAsync (ct);

    var grouped = entries.GroupBy (x => x.OccurredAt.Date).OrderByDescending (g => g.Key).ToList ();

    var totalRecords = grouped.Count;

    var pagedItems = grouped.Skip (((int)pageParams.Page! - 1) * (int)pageParams.PageSize!)
     .Take ((int)pageParams.PageSize).ToList ();

    var pageInfo = new PagedInfo (pageNumber: (int)pageParams.Page, pageSize: (int)pageParams.PageSize,
      totalPages: (int)Math.Ceiling (totalRecords / (double)pageParams.PageSize), totalRecords: totalRecords);

    return Result.Success ((Items: (IReadOnlyList<IGrouping<DateTime, ReputationEntry>>)pagedItems, Page: pageInfo));
  }

  public async Task<Result<IReadOnlyList<IGrouping<DateTime, ReputationEntry>>>> GetMonthReputationAsync (
    Guid userId,
    CancellationToken ct) {
    var fromDate = DateTime.UtcNow.Date.AddDays (-30);

    var query = await _context.ReputationEntries
     .Where (x => x.UserId == userId && x.OccurredAt >= fromDate)
     .ToListAsync (ct);

    var grouped = query.GroupBy (x => x.OccurredAt.Date).ToList ();

    return Result.Success ((IReadOnlyList<IGrouping<DateTime, ReputationEntry>>)grouped);
  }

  private async Task<int> UpsertEffectDelta (
    Guid userId,
    Guid parentId,
    Guid sourceId,
    ReputationSourceType st,
    string kind,
    int delta,
    int version,
    Guid eventId,
    string sourceService,
    CancellationToken ct) {
    var eff = await _context.ReputationEffects.FirstOrDefaultAsync (
      x => x.UserId == userId && x.SourceId == sourceId && x.SourceType == st && x.EffectKind == kind, ct);

    Console.WriteLine ($"===== UpsertEffectDelta: {JsonSerializer.Serialize (eff)}");
    Console.WriteLine ($"===== delta: {delta}");

    //Console.WriteLine ($"===== Delta: {eff.Amount}");

    if (eff is null) {
      if (delta == 0) return 0;
      _context.ReputationEffects.Add (ReputationEffect.Create (userId, parentId, sourceId, st, kind, delta, version,
        eventId, sourceService));
      return delta;
    }
    else {
      var applied = eff.Apply (delta, eventId);
      if (applied) _context.ReputationEffects.Update (eff);
      return delta;
    }
  }

  private async Task<UserReputationChanged> ApplyDeltaToSummary (Guid userId, int delta, CancellationToken ct) {
    var reputationSummary = await _context.ReputationSummaries.FirstOrDefaultAsync (x => x.UserId == userId, ct);
    if (reputationSummary is null) {
      reputationSummary = new ReputationSummary {
        UserId = userId, Total = delta, Version = 0, UpdatedAt = DateTime.UtcNow
      };
      _context.ReputationSummaries.Add (reputationSummary);
      return new (userId, reputationSummary.Total, reputationSummary.UpdatedAt);
    }

    var old = reputationSummary.Total;
    reputationSummary.Total += delta;
    reputationSummary.Version++;
    reputationSummary.UpdatedAt = DateTime.UtcNow;
    return new (UserId: userId, NewReputation: reputationSummary.Total, OccurredAt: reputationSummary.UpdatedAt);
  }

  private async Task<ReputationEffect> GetOrCreateEffect (
    Guid userId,
    Guid parentId,
    Guid sourceId,
    ReputationSourceType sourceType,
    string effectKind,
    string sourceService,
    CancellationToken ct) {
    var eff = await _context.ReputationEffects.FirstOrDefaultAsync (
      x => x.UserId == userId && x.SourceId == sourceId && x.SourceType == sourceType && x.EffectKind == effectKind,
      ct);

    if (eff is null) {
      eff = ReputationEffect.Create (userId, parentId, sourceId, sourceType, effectKind, 0, version: 0, Guid.Empty,
        sourceService);
      _context.ReputationEffects.Add (eff);
      await _context.SaveChangesAsync (ct);
    }

    return eff;
  }

}