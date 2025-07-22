using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository {

    private readonly CommentServiceDbContext _commentDbContext;

    public CommentRepository( CommentServiceDbContext commentDbContext ) {
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


    /// <summary> Получить список комментариев комментарий. </summary>
    public async Task<Result<IEnumerable<Comment>>> GetCommentsAsync( 
        TypeTarget type, Guid targetId, CancellationToken token ) {

        if(targetId == Guid.Empty)
            return Result<IEnumerable<Comment>>.Error("ID комментария не может быть пустым");

        List<Comment> comments = await _commentDbContext.Comments.Where(c => c.Type == type && c.TargetId == targetId).ToListAsync();

        return Result<IEnumerable<Comment>>.Success(comments);
    }


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

    /// <summary> Обновить комментарий. </summary>
    public async Task<Result> UpdateCommentAsync( Comment comment, CancellationToken token ) {

        if(comment is null)
            return Result.Error("Аргумент запроса не может быть null");

        try {
            _commentDbContext.Comments.Update(comment);
            await _commentDbContext.SaveChangesAsync( token);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Комментарий был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }

    }

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
