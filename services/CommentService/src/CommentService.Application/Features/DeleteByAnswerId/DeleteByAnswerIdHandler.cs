using Abstractions.Commands;
using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;

namespace CommentService.Application.Features.DeleteByAnswerId;

public class DeleteByAnswerIdHandler : ICommandHandler<DeleteByAnswerIdCommand> {

    private readonly ICommentRepository _commentRepository;

    public DeleteByAnswerIdHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( DeleteByAnswerIdCommand command, CancellationToken token ) {

        if(command.AnswerId == Guid.Empty)
            return Result.Error("Answer ID cannot be empty.");

        Result<IEnumerable<Comment>> resultComments = 
            await _commentRepository.GetCommentsAsync(TypeTarget.Answer, command.AnswerId, token);
        if(!resultComments.IsSuccess)
            return Result.Error(new ErrorList(resultComments.Errors));

        Result result = await _commentRepository.DeleteAnswerCommentsAsync(resultComments.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
