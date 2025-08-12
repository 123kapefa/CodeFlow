using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;

namespace CommentService.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository {

    private readonly CommentServiceDbContext _commentDbContext;
    private readonly ILogger<CommentRepository> _logger;

    public CommentRepository( CommentServiceDbContext commentDbContext, ILogger<CommentRepository> logger ) {
        _commentDbContext = commentDbContext;
        _logger = logger;
    }


    /// <summary> Получить комментарий по ID. </summary>
    public async Task<Result<Comment>> GetCommentByIdAsync( Guid commentId, CancellationToken token ) {

        _logger.LogInformation("GetCommentByIdAsync started. CommentId: {CommentId}", commentId);

        if(commentId == Guid.Empty) {
            _logger.LogWarning("GetCommentByIdAsync: пустой CommentId");
            return Result<Comment>.Error("ID комментария не может быть пустым");
        }

        Comment? comment = await _commentDbContext.Comments.FindAsync(commentId);

        if(comment == null) {
            _logger.LogWarning("GetCommentByIdAsync: комментарий {CommentId} не найден", commentId);
            return Result<Comment>.NotFound("Комментарий не найден");
        }

        _logger.LogInformation("GetCommentByIdAsync: комментарий {CommentId} успешно найден", commentId);
        return Result<Comment>.Success(comment);
    }
    

    /// <summary> Получить список комментариев. </summary>
    public async Task<Result<IEnumerable<Comment>>> GetCommentsAsync(
        TypeTarget type, Guid targetId, CancellationToken token ) {

        _logger.LogInformation("GetCommentsAsync started. Type: {Type}, TargetId: {TargetId}", type, targetId);

        if(targetId == Guid.Empty) {
            _logger.LogWarning("GetCommentsAsync: пустой TargetId");
            return Result<IEnumerable<Comment>>.Error("ID комментария не может быть пустым");
        }

        List<Comment> comments = await _commentDbContext.Comments
            .Where(c => c.Type == type && c.TargetId == targetId)
            .ToListAsync();

        _logger.LogInformation("GetCommentsAsync: получено {Count} комментариев для TargetId {TargetId}", comments.Count, targetId);
        return Result<IEnumerable<Comment>>.Success(comments);
    }

    public async Task<Result<IEnumerable<Comment>>> GetCommentsByAnswerIdsAsync (IEnumerable<Guid> answerIds, CancellationToken token) {
        try {
            _logger.LogInformation ("Поиск комментов по списку Id ответов.");
            await _commentDbContext.Comments
               .Where (comment => answerIds.Contains (comment.Id))
               .ToListAsync (token);
            
            _logger.LogInformation ("Комменты найдены.");
            return Result.Success ();
        }
        catch (Exception) {
            _logger.LogError ("База данных не отвечает");
            return Result.Error ("База данных не отвечает.");
        }
    }

    /// <summary> Создать комментарий. </summary>
    public async Task<Result> CreateCommentAsync( Comment comment, CancellationToken token ) {

        _logger.LogInformation("CreateCommentAsync started. CommentId: {CommentId}", comment?.Id);

        if(comment == null) {
            _logger.LogWarning("CreateCommentAsync: аргумент comment равен null");
            return Result.Error("Аргумент запроса не может быть null.");
        }

        try {
            await _commentDbContext.Comments.AddAsync(comment, token);
            await _commentDbContext.SaveChangesAsync(token);

            _logger.LogInformation("CreateCommentAsync: комментарий {CommentId} успешно создан", comment.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateCommentAsync: ошибка конкуренции при создании комментария {CommentId}", comment.Id);
            return Result.Error("Ошибка во время создания комментария.");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateCommentAsync: ошибка БД при создании комментария {CommentId}", comment.Id);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновить комментарий. </summary>
    public async Task<Result> UpdateCommentAsync( Comment comment, CancellationToken token ) {

        _logger.LogInformation("UpdateCommentAsync started. CommentId: {CommentId}", comment?.Id);

        if(comment is null) {
            _logger.LogWarning("UpdateCommentAsync: аргумент comment равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            _commentDbContext.Comments.Update(comment);
            await _commentDbContext.SaveChangesAsync(token);

            _logger.LogInformation("UpdateCommentAsync: комментарий {CommentId} успешно обновлён", comment.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "UpdateCommentAsync: комментарий {CommentId} был изменён или удалён другим пользователем", comment.Id);
            return Result.Error("Комментарий был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "UpdateCommentAsync: ошибка БД при обновлении комментария {CommentId}", comment.Id);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить комментарий. </summary>
    public async Task<Result> DeleteCommentByIdAsync( Guid commentId, CancellationToken token ) {

        _logger.LogInformation("DeleteCommentByIdAsync started. CommentId: {CommentId}", commentId);

        Comment? comment = await _commentDbContext.Comments.FindAsync(commentId);

        if(comment is null) {
            _logger.LogWarning("DeleteCommentByIdAsync: комментарий {CommentId} не найден", commentId);
            return Result.Error("Комментарий не найден");
        }

        try {
            _commentDbContext.Comments.Remove(comment);
            await _commentDbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteCommentByIdAsync: комментарий {CommentId} успешно удалён", commentId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "DeleteCommentByIdAsync: комментарий {CommentId} был изменён или удалён другим пользователем", commentId);
            return Result.Error("Контент был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteCommentByIdAsync: ошибка БД при удалении комментария {CommentId}", commentId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить всех комментариев пользователя по UserId. </summary>
    public async Task<Result> DeleteAllUserCommentsAsync( Guid userId, CancellationToken token ) {

        _logger.LogInformation("DeleteAllUserCommentsAsync started. UserId: {userId}", userId);

        List<Comment> comments = await _commentDbContext.Comments
            .Where(c => c.AuthorId == userId)
            .ToListAsync(token);

        try {
            _commentDbContext.Comments.RemoveRange(comments);
            await _commentDbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteAllUserCommentsAsync: комментарии пользователя {userId} успешно удалены", userId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(
                ex, 
                "DeleteAllUserCommentsAsync: комментарии пользователя {userId} были изменены или удалены другим пользователем", 
                userId);
            return Result.Error("Контент был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(
                ex,
                "DeleteAllUserCommentsAsync: ошибка БД при удалении комментариев {userId}", 
                userId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить комментарии для вопроса. </summary>
    public async Task<Result> DeleteAnswerCommentsAsync( IEnumerable<Comment> comments , CancellationToken token) {

        _logger.LogInformation("DeleteAnswerCommentsAsync started. Count: {Count}", comments.Count());

        _commentDbContext.Comments.RemoveRange(comments);
        await _commentDbContext.SaveChangesAsync(token);

        _logger.LogInformation("DeleteAnswerCommentsAsync: комментарии для ответа успешно удалены");
        return Result.Success();
    }

}
