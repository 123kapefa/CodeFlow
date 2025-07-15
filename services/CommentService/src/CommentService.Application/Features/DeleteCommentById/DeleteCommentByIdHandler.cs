using Ardalis.Result;
using CommentService.Domain.Repositories;
using Contracts.Commands;

namespace CommentService.Application.Features.DeleteCommentById;

public class DeleteCommentByIdHandler : ICommandHandler<DeleteCommentByIdCommand> {

    private readonly ICommentRepository _commentRepository;

    public DeleteCommentByIdHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( DeleteCommentByIdCommand command, CancellationToken token ) {

        Result result = await _commentRepository.DeleteCommentByIdAsync(command.CommentId, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
