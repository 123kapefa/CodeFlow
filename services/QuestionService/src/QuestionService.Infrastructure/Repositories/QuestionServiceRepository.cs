using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Filters;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Extensions;

namespace QuestionService.Infrastructure.Repositories;

public class QuestionServiceRepository : IQuestionServiceRepository {

    private readonly QuestionServiceDbContext _dbContext;

    public QuestionServiceRepository( QuestionServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получить вопрос.(не полный) </summary>
    public async Task<Result<Question>> GetQuestionShortAsync( Guid questionId, CancellationToken token ) {
        Question? question = await _dbContext.Questions.FindAsync(questionId, token);

        return question != null ?
            Result<Question>.Success(question) : Result<Question>.Error("Вопрос не найден");
    }


    /// <summary> Получить вопрос.(полный) </summary>
    public async Task<Result<Question>> GetQuestionAsync( Guid questionId, CancellationToken token ) {
        Question? question = await _dbContext.Questions
            .Include(q => q.QuestionChangingHistories)
            .Include(q => q.QuestionTags)
            .FirstOrDefaultAsync(q => q.Id == questionId, token);

        return question != null ?
            Result<Question>.Success(question) : Result<Question>.Error("Вопрос не найден");
    }
    


    /// <summary> Получить список вопросов. </summary>
    public async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetQuestionsAsync( 
        PageParams pageParams, 
        SortParams sortParams, 
        CancellationToken token ) {

        try {

            var users = await _dbContext.Questions  
                .Sort(sortParams)                
                .ToPagedAsync(pageParams);


            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Success(users);
        }
        catch(Exception) {
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }

    /// <summary> Получить список вопросов пользователя. </summary>
    public async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetUserQuestionsAsync( 
        Guid userId, 
        PageParams pageParams, 
        SortParams sortParams,
        CancellationToken token ) {

        try {

            var users = await _dbContext.Questions
                .Where(q => q.UserId == userId)
                .Sort(sortParams)
                .ToPagedAsync(pageParams);


            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Success(users);
        }
        catch(Exception) {
            return Result<(IEnumerable<Question> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }




    /// <summary> Получить историю изменений вопроса. </summary>
    public async Task<Result<IEnumerable<QuestionChangingHistory>>> GetQuestionChangingHistoryAsync(
        Guid questionId,
        CancellationToken token ) {

        try {
            List<QuestionChangingHistory> questionHistory = await _dbContext.QuestionChangingHistories
           .Where(q => q.QuestionId == questionId)
           .ToListAsync(token);

            return Result<IEnumerable<QuestionChangingHistory>>.Success(questionHistory);
        }
        catch(Exception) {
            return Result<IEnumerable<QuestionChangingHistory>>.Error("Ошибка базы данных при получении истории изменений вопросов");
        }

    }


    /// <summary> Получить теги вопроса. </summary>
    public async Task<Result<IEnumerable<QuestionTag>>> GetQuestionTagsAsync( Guid questionId, CancellationToken token ) {
        try {
            List<QuestionTag> questionTags = await _dbContext.QuestionTags
             .Where(qt => qt.QuestionId == questionId)
             .ToListAsync(token);

            return Result<IEnumerable<QuestionTag>>.Success(questionTags);
        }
        catch(Exception) {
            return Result<IEnumerable<QuestionTag>>.Error("Ошибка базы данных при получении тэгов вопросов");
        }

    }


    /// <summary> Создать вопрос. </summary>
    public async Task<Result> CreateQuestionAsync( Question question, CancellationToken token ) {

        if(question == null)
            return Result.Error("Аргумент запроса не может быть null");

        try {
            await _dbContext.Questions.AddAsync(question, token);
            await _dbContext.SaveChangesAsync(token);
            return Result.Success();
        }
        catch(DbUpdateException) {
            return Result.Error($"Ошибка БД");
        }
    }

  
    /// <summary> Создать историю изменений вопроса. </summary>
    public async Task<Result> CreateQuestionChangingHistoryAsync(
        QuestionChangingHistory questionChangingHistory,
        CancellationToken cancellationToken ) {

        if(questionChangingHistory == null)
            return Result.Error("Аргумент запроса не может быть null");

        try {
            await _dbContext.QuestionChangingHistories.AddAsync(questionChangingHistory, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Ошибка во время создания истории вопроса");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }

    }


    /// <summary> Обновление вопроса </summary>
    public async Task<Result> UpdateQuestionAsync( Question question, CancellationToken token ) {
        if(question == null)
            return Result.Error("Аргумент запроса не может быть null");


        try {           
            _dbContext.Questions.Update(question);
            await _dbContext.SaveChangesAsync(token);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Вопрос был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновление тэгов вопроса </summary>
    public async Task<Result> UpdateQuestionTagsAsync( Guid questionId, List<QuestionTag> questionTags, CancellationToken token ) {
        Result<IEnumerable<QuestionTag>> res = await GetQuestionTagsAsync(questionId, token);
        _dbContext.QuestionTags.RemoveRange(res.Value);

        foreach(QuestionTag questionTag in questionTags)
            _dbContext.QuestionTags.Add(questionTag);

        try {
            await _dbContext.SaveChangesAsync(token);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }     
        
    }


    /// <summary> Удалить вопрос.(каскадно) </summary>
    public async Task<Result> DeleteQuestionAsync( Guid questionId, CancellationToken token ) {
        if(questionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");

        Question? question = await _dbContext.Questions.FindAsync(questionId);

        if(question == null)
            return Result.Error("Вопрос с таким ID не найден");

        try {
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync(token);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Вопрос был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }

    }

}
