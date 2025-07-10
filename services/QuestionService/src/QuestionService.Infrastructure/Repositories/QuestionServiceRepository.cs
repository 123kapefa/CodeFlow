using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Filters;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Infrastructure.Repositories;
public class QuestionServiceRepository : IQuestionServiceRepository {

    private readonly QuestionServiceDbContext _dbContext;

    public QuestionServiceRepository( QuestionServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }

    /// <summary> Получить вопрос.(не полный) </summary>
    public async Task<Result<Question>> GetQuestionShortAsync( Guid questionId, CancellationToken token ) {
        Question? question = await _dbContext.Questions.FindAsync(questionId);

        return question != null ? 
            Result<Question>.Success(question) : Result<Question>.Error("Вопрос не найден"); 
    }

    /// <summary> Получить вопрос.(полный) </summary>
    public async Task<Result<Question>> GetQuestionAsync( Guid questionId, CancellationToken token ) {
        Question? question = await _dbContext.Questions
            .Include(q => q.QuestionChangingHistories)
            .Include(q => q.QuestionTags)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        return question != null ?
            Result<Question>.Success(question) : Result<Question>.Error("Вопрос не найден");
    }



    /// <summary> Получить список вопросов. </summary>
    public Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetQuestionsAsync( PageParams pageParams, SortParams sortParams, CancellationToken token ) => throw new NotImplementedException();

    /// <summary> Получить список вопросов пользователя. </summary>
    public Task<Result<IEnumerable<Question>>> GetUserQuestionsAsync( Guid userId, CancellationToken token ) => throw new NotImplementedException();




    /// <summary> Получить историю изменений вопроса. </summary>
    public async Task<Result<IEnumerable<QuestionChangingHistory>>> GetQuestionChangingHistoryAsync( 
        Guid questionId, 
        CancellationToken token ) {

        try {
            List<QuestionChangingHistory> questionHistory = await _dbContext.QuestionChangingHistories
           .Where(q => q.QuestionId == questionId)
           .ToListAsync();

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
             .ToListAsync();

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

        await _dbContext.Questions.AddAsync( question );
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary> Создать историю изменений вопроса. </summary>
    public async Task<Result> CreateQuestionChangingHistoryAsync( 
        QuestionChangingHistory questionChangingHistory, 
        CancellationToken cancellationToken ) {

        if(questionChangingHistory == null)
            return Result.Error("Аргумент запроса не может быть null");

        await _dbContext.QuestionChangingHistories.AddAsync( questionChangingHistory );
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary> Создать тэг вопроса. </summary>
    public async Task<Result> CreateQuestionTagsAsync( QuestionTag questionTag, CancellationToken token ) {
        if(questionTag == null)
            return Result.Error("Аргумент запроса не может быть null");

        await _dbContext.QuestionTags.AddAsync( questionTag );
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary> Обновление вопроса </summary>
    public async Task<Result> UpdateQuestionAsync( Question question, CancellationToken token ) {
        if(question == null)
            return Result.Error("Аргумент запроса не может быть null");

        _dbContext.Questions.Update( question ); 
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary> Удалить вопрос.(каскадно) </summary>
    public async Task<Result> DeleteQuestionAsync( Guid questionId, CancellationToken token ) {
        if(questionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");

        Question? question = await _dbContext.Questions.FindAsync( questionId );

        if(question == null)
            return Result.Error("Вопрос с таким ID не найден");

        _dbContext.Questions.Remove( question );
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary> Удалить тэг вопроса. </summary>
    public async Task<Result> DeleteQuestionTagAsync( Guid questionId, int tagId, CancellationToken cancellationToken ) {
        if(questionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");


        QuestionTag? questionTag = await _dbContext.QuestionTags
            .FirstOrDefaultAsync(qt => qt.QuestionId == questionId);

        if(questionTag == null)
            return Result.Error("Тэг для вопроса с таким ID не найден");

        _dbContext .QuestionTags.Remove( questionTag );
        await _dbContext.SaveChangesAsync();

        return Result.Success();

    }


    /// <summary> Обновление вопроса.(правильный ответ) </summary>
    public async Task<Result> UpdateQuestionAcceptAsync( 
        Guid questionId, //TODO перенести в Handler
        Guid acceptedAnswerId, //TODO перенести в Handler
        Question acceptedQuestion,
        CancellationToken cancellationToken ) {

        if(questionId == Guid.Empty) //TODO перенести в Handler
            return Result.Error("ID вопроса не может быть пустым");

        if(acceptedAnswerId == Guid.Empty) //TODO перенести в Handler
            return Result.Error("ID ответа не может быть пустым");

        _dbContext.Questions.Update(acceptedQuestion);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    // TODO ПОДУМАТЬ!!! может использовать UpdateQuestionAcceptAsync для обновления вопроса
    /// <summary> Обновление вопроса.(просмотры) </summary>
    public async Task<Result> UpdateQuestionViewAsync( Guid questionId, CancellationToken cancellationToken ) => throw new NotImplementedException();
    
    // TODO ПОДУМАТЬ!!! может использовать UpdateQuestionAcceptAsync для обновления вопроса
    /// <summary> Обновление вопроса.(голосование) </summary>
    public async Task<Result> UpdateQuestionVoteAsync( Guid questionId, int upvotes, int downvotes, CancellationToken cancellationToken ) => throw new NotImplementedException();


    
}
