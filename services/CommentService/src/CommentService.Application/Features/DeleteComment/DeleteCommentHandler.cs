using Abstractions.Commands;

using Ardalis.Result;

using CommentService.Domain.Repositories;

namespace CommentService.Application.Features.DeleteComment;

public class DeleteCommentHandler : ICommandHandler<DeleteCommentCommand> {

    private readonly ICommentRepository _commentRepository;

    public DeleteCommentHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( DeleteCommentCommand command, CancellationToken token ) {

        Result result = await _commentRepository.DeleteCommentByIdAsync(command.CommentId, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
