using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Domain.Repositories;

public interface ICommentRepository {
    Task<Result<IEnumerable<Comment>>> GetQuestionCommentsAsync(
        TypeTarget target, Guid questionId, CancellationToken token);
    Task<Result<IEnumerable<Comment>>> GetAnswerCommentsAsync( 
        TypeTarget target, Guid answerId, CancellationToken token );
    Task<Result<Comment>> GetCommentByIdAsync( Guid commentId, CancellationToken token );

    Task<Result> CreateCommentAsync(Comment comment, CancellationToken token);
    Task<Result> UpdateCommentAsync( Comment comment, CancellationToken token );

    Task<Result> DeleteCommentByIdAsync( Guid commentId, CancellationToken token );
}
