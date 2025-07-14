using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository {

    private readonly CommentDbContext _commentDbContext;

    public CommentRepository( CommentDbContext commentDbContext ) {
        _commentDbContext = commentDbContext;
    }

    /// <summary> Получить комментарий по ID. </summary>
    public async Task<Result<Comment>> GetCommentByIdAsync( Guid commentId, CancellationToken token ) {

        if(commentId == Guid.Empty)
            return Result<Comment>.Error("ID комментария не может быть пустым");

        Comment? comment = await _commentDbContext.Comments.FindAsync(commentId);

        if(comment == null)
            return Result<Comment>.NotFound("Комментарий не найден");

        return Result<Comment>.Success(comment);
    }


    public Task<Result<IEnumerable<Comment>>> GetQuestionCommentsAsync( TypeTarget target, Guid questionId, CancellationToken token ) => throw new NotImplementedException();


    public Task<Result<IEnumerable<Comment>>> GetAnswerCommentsAsync( TypeTarget target, Guid answerId, CancellationToken token ) => throw new NotImplementedException();



    /// <summary> Создать комментарий. </summary>
    public async Task<Result> CreateCommentAsync( Comment comment, CancellationToken token ) {
        if(comment == null)
            return Result.Error("Аргумент запроса не может быть null.");

        try {
            await _commentDbContext.Comments.AddAsync(comment, token);
            await _commentDbContext.SaveChangesAsync(token);

            return Result.Success();
        }          
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Ошибка во время создания комментария.");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }

    }

    public Task<Result> UpdateCommentAsync( Comment comment, CancellationToken token ) => throw new NotImplementedException();


    /// <summary> Удалить комментарий. </summary>
    public async Task<Result> DeleteCommentByIdAsync( Guid commentId, CancellationToken token ) {

        Comment? comment = await _commentDbContext.Comments.FindAsync(commentId);

        if(comment is null)
            return Result.Error("Комментарий не найден");

        try {
            _commentDbContext.Comments.Remove(comment);
            await _commentDbContext.SaveChangesAsync(token);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Контент был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }

    }
   
}
