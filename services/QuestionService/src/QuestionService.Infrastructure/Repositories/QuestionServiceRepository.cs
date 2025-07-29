using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Filters;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Extensions;

namespace QuestionService.Infrastructure.Repositories;

public class QuestionServiceRepository : IQuestionServiceRepository {

    private readonly QuestionServiceDbContext _dbContext;
    private readonly ILogger<QuestionServiceRepository> _logger;

    public QuestionServiceRepository( QuestionServiceDbContext dbContext, ILogger<QuestionServiceRepository> logger ) {
        _dbContext = dbContext;
        _logger = logger;
    }


    /// <summary> Получить вопрос.(не полный) </summary>
    public async Task<Result<Question>> GetQuestionShortAsync( Guid questionId, CancellationToken token ) {

        _logger.LogInformation("GetQuestionShortAsync started. QuestionId: {QuestionId}", questionId);

        Question? question = await _dbContext.Questions.FindAsync(questionId, token);

        if(question == null) {
            _logger.LogWarning("GetQuestionShortAsync: вопрос {QuestionId} не найден", questionId);
            return Result<Question>.Error("Вопрос не найден");
        }

        _logger.LogInformation("GetQuestionShortAsync: вопрос {QuestionId} успешно найден", questionId);
        return Result<Question>.Success(question);
    }


    /// <summary> Получить вопрос.(полный) </summary>
    public async Task<Result<Question>> GetQuestionAsync( Guid questionId, CancellationToken token ) {

        _logger.LogInformation("GetQuestionAsync started. QuestionId: {QuestionId}", questionId);

        Question? question = await _dbContext.Questions
            .Include(q => q.QuestionChangingHistories)
            .Include(q => q.QuestionTags)
            .FirstOrDefaultAsync(q => q.Id == questionId, token);

        if(question == null) {
            _logger.LogWarning("GetQuestionAsync: вопрос {QuestionId} не найден", questionId);
            return Result<Question>.Error("Вопрос не найден");
        }

        _logger.LogInformation("GetQuestionAsync: вопрос {QuestionId} успешно получен", questionId);
        return Result<Question>.Success(question);
    }


    /// <summary> Получить список вопросов. </summary>
    public async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetQuestionsAsync(
        PageParams pageParams,
        SortParams sortParams,
        CancellationToken token ) {

        _logger.LogInformation("GetQuestionsAsync started. PageParams: {@PageParams}, SortParams: {@SortParams}", pageParams, sortParams);

        try {
            var users = await _dbContext.Questions
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            _logger.LogInformation("GetQuestionsAsync: получено {Count} вопросов", users.Value.items.Count());
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Success(users);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetQuestionsAsync: ошибка базы данных");
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Получить список вопросов пользователя. </summary>
    public async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetUserQuestionsAsync(
        Guid userId,
        PageParams pageParams,
        SortParams sortParams,
        CancellationToken token ) {

        _logger.LogInformation(
            "GetUserQuestionsAsync started. UserId: {UserId}, PageParams: {@PageParams}, SortParams: {@SortParams}",
            userId, pageParams, sortParams);

        try {
            var users = await _dbContext.Questions
                .Where(q => q.UserId == userId)
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            _logger.LogInformation(
                "GetUserQuestionsAsync: получено {Count} вопросов пользователя {UserId}", users.Value.items.Count(), userId);
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Success(users);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetUserQuestionsAsync: ошибка базы данных");
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Получить историю изменений вопроса. </summary>
    public async Task<Result<IEnumerable<QuestionChangingHistory>>> GetQuestionChangingHistoryAsync(
        Guid questionId,
        CancellationToken token ) {

        _logger.LogInformation("GetQuestionChangingHistoryAsync started. QuestionId: {QuestionId}", questionId);

        try {
            List<QuestionChangingHistory> questionHistory = await _dbContext.QuestionChangingHistories
                .Where(q => q.QuestionId == questionId)
                .ToListAsync(token);

            _logger.LogInformation("GetQuestionChangingHistoryAsync: получено {Count} записей истории вопроса {QuestionId}", questionHistory.Count, questionId);
            return Result<IEnumerable<QuestionChangingHistory>>.Success(questionHistory);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetQuestionChangingHistoryAsync: ошибка базы данных");            
            return Result<IEnumerable<QuestionChangingHistory>>.Error(
                "Ошибка базы данных при получении истории изменений вопросов");
        }
    }


    /// <summary> Получить теги вопроса. </summary>
    public async Task<Result<IEnumerable<QuestionTag>>> GetQuestionTagsAsync( Guid questionId, CancellationToken token ) {

        _logger.LogInformation("GetQuestionTagsAsync started. QuestionId: {QuestionId}", questionId);

        try {
            List<QuestionTag> questionTags = await _dbContext.QuestionTags
                .Where(qt => qt.QuestionId == questionId)
                .ToListAsync(token);

            _logger.LogInformation("GetQuestionTagsAsync: получено {Count} тэгов для вопроса {QuestionId}", questionTags.Count, questionId);
            return Result<IEnumerable<QuestionTag>>.Success(questionTags);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetQuestionTagsAsync: ошибка базы данных");
            return Result<IEnumerable<QuestionTag>>.Error("Ошибка базы данных при получении тэгов вопросов");

        }
    }


    /// <summary> Создать вопрос. </summary>
    public async Task<Result> CreateQuestionAsync( Question question, CancellationToken token ) {

        _logger.LogInformation("CreateQuestionAsync started. QuestionId: {QuestionId}", question?.Id);

        if(question == null) {
            _logger.LogWarning("CreateQuestionAsync: аргумент вопроса равен null");

            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            await _dbContext.Questions.AddAsync(question, token);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("CreateQuestionAsync: вопрос {QuestionId} успешно создан", question.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateQuestionAsync: ошибка конкуренции при создании вопроса {QuestionId}", question.Id);
            return Result.Error("Ошибка во время создания истории вопроса");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateQuestionAsync: ошибка БД при создании вопроса {QuestionId}", question.Id);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Создать историю изменений вопроса. </summary>
    public async Task<Result> CreateQuestionChangingHistoryAsync(
        QuestionChangingHistory questionChangingHistory,
        CancellationToken cancellationToken ) {

        _logger.LogInformation(
            "CreateQuestionChangingHistoryAsync started. QuestionId: {QuestionId}", questionChangingHistory?.QuestionId);

        if(questionChangingHistory == null) {
            _logger.LogWarning("CreateQuestionChangingHistoryAsync: аргумент истории равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            await _dbContext.QuestionChangingHistories.AddAsync(questionChangingHistory, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "CreateQuestionChangingHistoryAsync: история изменений для вопроса {QuestionId} создана", questionChangingHistory.QuestionId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateQuestionChangingHistoryAsync: ошибка конкуренции при создании истории вопроса {QuestionId}", questionChangingHistory.QuestionId);
            return Result.Error("Ошибка во время создания истории изменений вопроса");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateQuestionChangingHistoryAsync: ошибка БД при создании истории вопроса {QuestionId}", questionChangingHistory.QuestionId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновление вопроса </summary>
    public async Task<Result> UpdateQuestionAsync( Question question, CancellationToken token ) {

        _logger.LogInformation("UpdateQuestionAsync started. QuestionId: {QuestionId}", question?.Id);

        if(question == null) {
            _logger.LogWarning("UpdateQuestionAsync: аргумент вопроса равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            _dbContext.Questions.Update(question);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("UpdateQuestionAsync: вопрос {QuestionId} успешно обновлён", question.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "UpdateQuestionAsync: вопрос {QuestionId} был изменён или удалён другим пользователем", question.Id);
            return Result.Error("Вопрос был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "UpdateQuestionAsync: ошибка БД при обновлении вопроса {QuestionId}", question.Id);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновление тэгов вопроса </summary>
    public async Task<Result> UpdateQuestionTagsAsync( Guid questionId, List<QuestionTag> questionTags, CancellationToken token ) {

        _logger.LogInformation("UpdateQuestionTagsAsync started. QuestionId: {QuestionId}", questionId);

        Result<IEnumerable<QuestionTag>> res = await GetQuestionTagsAsync(questionId, token);
        _dbContext.QuestionTags.RemoveRange(res.Value);

        foreach(QuestionTag questionTag in questionTags)
            _dbContext.QuestionTags.Add(questionTag);

        try {
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("UpdateQuestionTagsAsync: тэги вопроса {QuestionId} успешно обновлены", questionId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "UpdateQuestionTagsAsync: тэги вопроса {QuestionId} были изменены или удалены другим пользователем", questionId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "UpdateQuestionTagsAsync: ошибка БД при обновлении тэгов вопроса {QuestionId}", questionId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить вопрос.(каскадно) </summary>
    public async Task<Result> DeleteQuestionAsync( Guid questionId, CancellationToken token ) {

        _logger.LogInformation("DeleteQuestionAsync started. QuestionId: {QuestionId}", questionId);

        if(questionId == Guid.Empty) {
            _logger.LogWarning("DeleteQuestionAsync: пустой QuestionId");
            return Result.Error("ID вопроса не может быть пустым");
        }

        Question? question = await _dbContext.Questions.FindAsync(questionId);

        if(question == null) {
            _logger.LogWarning("DeleteQuestionAsync: вопрос {QuestionId} не найден", questionId);
            return Result.Error("Вопрос с таким ID не найден");
        }

        try {
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteQuestionAsync: вопрос {QuestionId} успешно удалён", questionId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "DeleteQuestionAsync: вопрос {QuestionId} был изменён или удалён другим пользователем", questionId);
            return Result.Error("Вопрос был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteQuestionAsync: ошибка БД при удалении вопроса {QuestionId}", questionId);
            return Result.Error("Ошибка БД");
        }
    }

    public async Task SaveChangesAsync( CancellationToken token ) {
        await _dbContext.SaveChangesAsync();
    }

}
