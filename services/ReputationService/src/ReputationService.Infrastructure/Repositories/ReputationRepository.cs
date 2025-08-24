using System.Text.Json;

using Ardalis.Result;

using Contracts.Common.Filters;
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

  public async Task<Result<ReputationEntry>> AppendEntryAsync (ReputationEntry entry, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.AppendEntryAsync: Добавление новой записи об изменении репутации пользователя.");

      await _context.ReputationEntries.AddAsync (entry, ct);

      _logger.LogInformation ($"ReputationRepository.AppendEntryAsync: Новая запись успешно добавлена.");
      return Result<ReputationEntry>.Success (entry);
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.AppendEntryAsync: отмена операции.");
      return Result<ReputationEntry>.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.AppendEntryAsync: База данных не отвечает");
      return Result<ReputationEntry>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<IReadOnlyList<ReputationEntry>>> AppendEntriesAsync (
    IReadOnlyCollection<ReputationEntry> entries,
    CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.AppendEntriesAsync: Добавление новых записей об изменении репутации пользователя.");

      await _context.ReputationEntries.AddRangeAsync (entries, ct);

      _logger.LogInformation ($"ReputationRepository.AppendEntriesAsync: Новые записи успешно добавлены.");
      return Result<IReadOnlyList<ReputationEntry>>.Success (entries.ToList ());
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.AppendEntriesAsync: отмена операции.");
      return Result<IReadOnlyList<ReputationEntry>>.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.AppendEntriesAsync: База данных не отвечает");
      return Result<IReadOnlyList<ReputationEntry>>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<ReputationEntry>> GetEntryAsync (Guid id, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.GetEntryAsync: Получение записи об изменении репутации пользователя.");

      var reputationEntry = await _context.ReputationEntries.FirstOrDefaultAsync (re => re.Id == id, ct);

      if (reputationEntry is null) {
        _logger.LogInformation (
          $"ReputationRepository.GetEntryAsync: Запись об изменении репутации пользователя не найдены.");
        return Result<ReputationEntry>.NotFound ();
      }

      _logger.LogInformation (
        $"ReputationRepository.GetEntryAsync: Получение записей о изменении репутации пользователя.");
      return Result.Success ();
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.GetEntryAsync: отмена операции.");
      return Result.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.GetEntryAsync: База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<IReadOnlyList<ReputationEntry>>> GetEntriesAsync (Guid userId, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.GetEntriesAsync: Добавление новой записи о изменении репутации пользователя.");

      var reputationEntries = await _context.ReputationEntries.Where (re => re.UserId == userId).ToListAsync (ct);

      if (!reputationEntries.Any ()) {
        _logger.LogInformation (
          $"ReputationRepository.GetEntryAsync: Записи об изменении репутации пользователя не найдены.");
        return Result<IReadOnlyList<ReputationEntry>>.NotFound ();
      }

      _logger.LogInformation ($"ReputationRepository.GetEntriesAsync: Новая запись успешно добавлена.");
      return Result<IReadOnlyList<ReputationEntry>>.Success (reputationEntries);
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.GetEntriesAsync: отмена операции.");
      return Result<IReadOnlyList<ReputationEntry>>.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.GetEntriesAsync: База данных не отвечает");
      return Result<IReadOnlyList<ReputationEntry>>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>> GetPageEntriesAsync (
    Guid userId,
    PageParams pageParams,
    SortParams sortParams,
    CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.GetPageEntriesAsync: Поиск записей об изменении репутации пользователя с таким ID: {userId}.",
        userId);
      var reputationEntries = await _context.ReputationEntries.Where (a => a.UserId == userId).Sort (sortParams)
       .ToPagedAsync (pageParams);

      _logger.LogInformation (
        "ReputationRepository.GetPageEntriesAsync: Возврат записей данного пользователя с таким ID: {userId} с количеством {count}.",
        userId, reputationEntries.Value.items.Count ());
      return Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>.Success (reputationEntries);
    }
    catch (OperationCanceledException) {
      _logger.LogError ("ReputationRepository.GetPageEntriesAsync: Отмена операции.");
      return Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>.Error ("База данных не отвечает.");
    }
    catch (Exception) {
      _logger.LogError ("ReputationRepository.GetPageEntriesAsync: База данных не отвечает");
      return Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result> DeleteEntryAsync (Guid id, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.DeleteEntryAsync: Удаление записи об изменении репутации пользователя.");

      var reputationEntry = await _context.ReputationEntries.FirstOrDefaultAsync (re => re.Id == id, ct);

      if (reputationEntry is null) {
        _logger.LogInformation ("ReputationRepository.DeleteEntryAsync: Такой записи с ID: {Id} не найдено.", id);

        return Result.NotFound ();
      }

      _context.ReputationEntries.Remove (reputationEntry);

      _logger.LogInformation ($"ReputationRepository.DeleteEntryAsync: Новая запись успешно удалена.");
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

  public async Task<Result> DeleteEntriesAsync (Guid userId, CancellationToken ct) {
    try {
      _logger.LogInformation (
        "ReputationRepository.DeleteEntryAsync: Удаление записей об изменении репутации пользователя.");

      var reputationEntries = await GetEntriesAsync (userId, ct);

      _context.ReputationEntries.RemoveRange ((List<ReputationEntry>)reputationEntries);

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
          _context.ReputationEntries.Add (ReputationEntry.Create (newId, parentId, newAnswerId, ReputationSourceType.Answer,
            "AcceptedAnswer", reason, newDelta, occurredAt, eventId, sourceService));
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

  public async Task<IReadOnlyList<ReputationEffect>> GetEffectsAsync (Guid userId, CancellationToken ct) {
    var reputationEffects = await _context.ReputationEffects.AsNoTracking ()
     .Where (x => x.UserId == userId && x.Amount != 0).OrderByDescending (x => x.UpdatedAt).ToListAsync (ct);
    return reputationEffects;
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
      _context.ReputationEffects.Add (ReputationEffect.Create (userId, parentId, sourceId, st, kind, delta, version, eventId,
        sourceService));
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
  
  private async Task<ReputationEffect> GetOrCreateEffect(
    Guid userId,
    Guid parentId,
    Guid sourceId,
    ReputationSourceType sourceType,
    string effectKind,
    string sourceService,
    CancellationToken ct)
  {
    var eff = await _context.ReputationEffects
     .FirstOrDefaultAsync(x =>
        x.UserId == userId &&
        x.SourceId == sourceId &&
        x.SourceType == sourceType &&
        x.EffectKind == effectKind, ct);

    if (eff is null)
    {
      eff = ReputationEffect.Create(userId, parentId, sourceId, sourceType, effectKind, 0, version: 0, Guid.Empty, sourceService);
      _context.ReputationEffects.Add(eff);
      await _context.SaveChangesAsync(ct);
    }

    return eff;
  }

}