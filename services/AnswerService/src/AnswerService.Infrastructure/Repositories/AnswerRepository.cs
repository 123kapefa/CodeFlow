using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnswerService.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository {

  private readonly AnswerServiceDbContext _context;
  private readonly ILogger<AnswerRepository> _logger;

  public AnswerRepository (AnswerServiceDbContext context, ILogger<AnswerRepository> logger) {
    _context = context;
    _logger = logger;
  }

  public async Task<Result<IEnumerable<Answer>>> GetByQuestionIdAsync (Guid questionId, CancellationToken ct) {
    try {
      _logger.LogInformation ("Получение списка ответов по {QuestionId}.", questionId);
      var answers = await _context.Answers
       .Where (a => a.QuestionId == questionId).ToListAsync (ct);
      
      if (answers.Any ()) {
        _logger.LogError ("Ответы не найдены.");
        Result<IEnumerable<Answer>>.Error ("Ответы не найдены.");
      }
      
      _logger.LogInformation ("Возврат найденных ответов.");
      return Result<IEnumerable<Answer>>.Success (answers);
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result<IEnumerable<Answer>>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<Answer>> GetByIdAsync (Guid id, CancellationToken ct) {
    try {
      _logger.LogInformation ("Получение ответа по ID: {id}.", id);
      var answer = await _context.Answers
       .FirstOrDefaultAsync (a => a.Id == id, ct);
      
      if (answer is null) {
        _logger.LogInformation ("Ответ не найден.");
        return Result<Answer>.Error ("Ответ не найден.");
      }
      
      _logger.LogDebug ("Возврат найденного ответа.");
      return Result<Answer>.Success (answer);
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result<Answer>.Error ("База данных не отвечает.");
    }
  }
  
  public async Task<Result<Answer>> GetByIdAndQuestionIdAsync (Guid id, Guid questionId, CancellationToken ct) {
    try {
      _logger.LogInformation ("Получение ответа по ID: {id} QuestionId: {questionId}.", id, questionId);
      var answer = await _context.Answers
       .FirstOrDefaultAsync (a => a.Id == id && a.QuestionId == questionId, ct);
      
      if (answer is null) {
        _logger.LogInformation ("Ответ не найден.");
        return Result<Answer>.Error ("Ответ не найден.");
      }

      _logger.LogInformation ("Возврат найденного ответа.");
      return Result<Answer>.Success (answer);
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result<Answer>.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result> AddAsync (Answer answer, CancellationToken ct) {
    try {
      _logger.LogInformation ("Добавление ответа");
      answer.CreatedAt = DateTime.UtcNow;
      await _context.Answers.AddAsync (answer, ct);
      await _context.SaveChangesAsync (ct);
      
      _logger.LogInformation ("Ответ успешно добавлен.");
      return Result.Success ();
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }
  
  public async Task<Result> UpdateAsync (Answer answer, CancellationToken ct) {
    try { 
      _logger.LogInformation ("Обновление ответа.");
      _context.Answers.Update(answer);
      await _context.SaveChangesAsync(ct);
      
      _logger.LogInformation ("Ответ успешно обновлен.");
      return Result.Success();
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }
  
  public async Task<Result> DeleteAsync (Answer answer, CancellationToken ct) {
    try {
      _logger.LogInformation ("Удаление ответа.");
      _context.Answers.Remove(answer);
      await _context.SaveChangesAsync(ct);
      
      _logger.LogInformation ("Ответ успешно обновлен.");
      return Result.Success();
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result> AcceptAsync (IEnumerable<Answer> answers, Guid answerId, CancellationToken ct) {
    try {
      _logger.LogInformation ("Поиск правильно ответа.");
      foreach (var answer in answers) {
        if (answer.Id == answerId) {
          answer.IsAccepted = true;
        }
        
        answer.IsAccepted = false;
      }
      await _context.SaveChangesAsync(ct);
      
      _logger.LogInformation ("Правильный ответ выбран.");
      return Result.Success();
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result.Error ("База данных не отвечает.");
    }
  }

  public async Task<Result<IEnumerable<Answer>>> GetByUserIdAsync (Guid userId, CancellationToken ct) {
    try {
      _logger.LogInformation ("Поиск ответов пользователя с таким ID: {userId}.", userId);
      var answers = await _context.Answers
       .Where(a => a.UserId == userId)
       .ToListAsync(ct);

      if (answers.Count == 0) {
        _logger.LogError ("Ответы данного пользователя c таким ID: {userId} не найдены.", userId);
        return Result<IEnumerable<Answer>>.Error("Ответы данного пользователя не найдены.");
      }

      _logger.LogInformation ("Возврат ответов данного пользователя с таким ID: {userId}.", userId);
      return Result<IEnumerable<Answer>>.Success(answers);
    } catch (Exception) {
      _logger.LogError ("База данных не отвечает");
      return Result<IEnumerable<Answer>>.Error ("База данных не отвечает.");
    }
  }

  // TODO добавить получение комментариев определенного ответа
  public async Task<Result<IEnumerable<string>>> GetCommentsByAnswerIdAsync (Guid userId, CancellationToken ct) {
    throw new Exception ("");
  }

  public async Task SaveAsync (CancellationToken ct) => await _context.SaveChangesAsync(ct);

}