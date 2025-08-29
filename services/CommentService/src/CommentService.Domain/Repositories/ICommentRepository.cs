using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;

namespace CommentService.Domain.Repositories;

public interface ICommentRepository {
    Task<Result<IEnumerable<Comment>>> GetCommentsAsync(
        TypeTarget target, Guid questionId, CancellationToken token);
    
    Task<Result<Comment>> GetCommentByIdAsync( Guid commentId, CancellationToken token );
    Task<Result<IEnumerable<Comment>>> GetCommentsByAnswerIdsAsync( IEnumerable<Guid> answerIds, CancellationToken token );

    Task<Result> CreateCommentAsync(Comment comment, CancellationToken token);
    Task<Result> UpdateCommentAsync( Comment comment, CancellationToken token );

    Task<Result> DeleteCommentByIdAsync( Guid commentId, CancellationToken token );
    Task<Result> DeleteAllUserCommentsAsync( Guid userId, CancellationToken token );
    Task<Result> DeleteAnswerCommentsAsync( IEnumerable<Comment> comments, CancellationToken token );
}
