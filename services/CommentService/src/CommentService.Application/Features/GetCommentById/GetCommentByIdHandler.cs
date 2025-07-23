using Abstractions.Commands;

using Ardalis.Result;

using CommentService.Domain.Entities;
using CommentService.Domain.Repositories;

using Contracts.CommentService.DTOs;

namespace CommentService.Application.Features.GetCommentById;

public class GetCommentByIdHandler : ICommandHandler<CommentDTO, GetCommentByIdCommand> {

    private readonly ICommentRepository _commentRepository;

    public GetCommentByIdHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result<CommentDTO>> Handle( GetCommentByIdCommand command, CancellationToken token ) {

        Result<Comment> result = await _commentRepository.GetCommentByIdAsync(command.CommentId, token);

        if(!result.IsSuccess)
            return Result.Error(new ErrorList(result.Errors));

        CommentDTO commentDTO = new CommentDTO {
            Id = result.Value.Id,
            AuthorId = result.Value.AuthorId,
            Content = result.Value.Content,
            CreatedAt = result.Value.CreatedAt,
            Type = Enum.GetName (result.Value.Type)!,
            TargetId = result.Value.TargetId
        };

        return Result.Success(commentDTO);
    }
}
