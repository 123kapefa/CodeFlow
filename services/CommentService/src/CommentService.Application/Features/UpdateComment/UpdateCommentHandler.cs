using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Repositories;
using Contracts.Commands;

namespace CommentService.Application.Features.UpdateComment;

public class UpdateCommentHandler : ICommandHandler<UpdateCommentCommand> {

    private readonly ICommentRepository _commentRepository;

    public UpdateCommentHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( UpdateCommentCommand command, CancellationToken token ) {

        Result<Comment> commentResult = await _commentRepository.GetCommentByIdAsync(command.CommentId, token);

        if(!commentResult.IsSuccess)
            return Result.Error(new ErrorList(commentResult.Errors));

        commentResult.Value.Content = command.Content;

        Result result = await _commentRepository.UpdateCommentAsync(commentResult.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
