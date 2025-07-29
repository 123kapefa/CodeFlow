using Abstractions.Commands;
using Ardalis.Result;
using CommentService.Domain.Repositories;

namespace CommentService.Application.Features.DeleteAllUserComments;

public class DeleteAllUserCommentsHandler : ICommandHandler<DeleteAllUserCommentsCommand> {

    private readonly ICommentRepository _commentRepository;

    public DeleteAllUserCommentsHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( DeleteAllUserCommentsCommand command, CancellationToken cancellationToken ) {

        if(command.UserId == Guid.Empty)
            return Result.Error("UserID не может быть пустым");

        Result result = 
            await _commentRepository.DeleteAllUserCommentsAsync(command.UserId, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}