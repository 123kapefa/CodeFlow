using Ardalis.Result;

using Contracts.Common.Filters;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReputationService.Domain.Entities;
using ReputationService.Domain.Repositories;
using ReputationService.Infrastructure.Extensions;

namespace ReputationService.Infrastructure.Repositories;

public sealed class ReputationRepository {

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

  public async Task<Result<IReadOnlyList<ReputationEntry>>> AppendEntriesAsync (IReadOnlyCollection<ReputationEntry> entries, CancellationToken ct) {
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

}