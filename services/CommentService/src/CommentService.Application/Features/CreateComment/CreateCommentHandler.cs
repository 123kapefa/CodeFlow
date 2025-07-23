using Abstractions.Commands;

using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;

namespace CommentService.Application.Features.CreateComment;

public class CreateCommentHandler : ICommandHandler<CreateCommentCommand> {

    private readonly ICommentRepository _commentRepository;

    public CreateCommentHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( CreateCommentCommand command, CancellationToken token ) {

        Comment comment = new Comment { 
            AuthorId = command.CreateCommentDTO.AuthorId,
            Content = command.CreateCommentDTO.Content,
            CreatedAt = DateTime.UtcNow,
            Type = Enum.Parse<TypeTarget>(command.CreateCommentDTO.Type),
            TargetId = command.CreateCommentDTO.TargetId,
        };

        Result result = await _commentRepository.CreateCommentAsync(comment, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
