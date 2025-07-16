using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnswerService.Infrastructure.Repositories;

public class AnswerChangingHistoryRepository : IAnswerChangingHistoryRepository {

  private readonly AnswerServiceDbContext _context;
  private readonly ILogger<AnswerChangingHistoryRepository> _logger;
  
  public AnswerChangingHistoryRepository (AnswerServiceDbContext context, ILogger<AnswerChangingHistoryRepository> logger) {
    _context = context;
    _logger = logger;
  }

  public async Task<Result> CreateAsync (AnswerChangingHistory answerChangingHistory, CancellationToken ct) {
    try {
      _logger.LogInformation ($"Создание новой записи об изменении контента для ответа {answerChangingHistory.Id}.", answerChangingHistory.Id);
      await _context.AnswerChangingHistories.AddAsync (answerChangingHistory, ct);
      await _context.SaveChangesAsync (ct);

      _logger.LogInformation ("Новая запись об изменении контента для ответа успешно добавлена.");
      return Result.Success ();
    }
    catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }
  
  public async Task<Result> DeleteAsync (Guid id, CancellationToken ct) {
    try {
      _logger.LogInformation ("Поиск изменения для ответа.");
      var answerChangingHistory = await _context.AnswerChangingHistories.
        FirstOrDefaultAsync (ach => ach.Id == id, ct);

      if (answerChangingHistory is null) {
        _logger.LogError ("Изменение для ответа не найдено.");
        return Result.Error("Такое изменение для ответа не найдено.");
      }
      
      _context.AnswerChangingHistories.Remove (answerChangingHistory);
      await _context.SaveChangesAsync (ct);
      
      _logger.LogInformation ("Изменение для ответа было успешно удалено.");
      return Result.Success ();
    }
    catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }

}