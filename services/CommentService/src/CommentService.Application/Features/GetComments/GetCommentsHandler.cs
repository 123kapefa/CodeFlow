using Abstractions.Commands;

using Ardalis.Result;

using CommentService.Domain.Entities;
using CommentService.Domain.Repositories;

using Contracts.DTOs.CommentService;

namespace CommentService.Application.Features.GetComments;

public class GetCommentsHandler : ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand> {

    private readonly ICommentRepository _commentRepository;

    public GetCommentsHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result<IEnumerable<CommentDTO>>> Handle( 
        GetCommentsCommand command, CancellationToken token ) {

        Result<IEnumerable<Comment>> result = 
            await _commentRepository.GetCommentsAsync(command.Type, command.TargetId, token);

        if(!result.IsSuccess)
            return Result<IEnumerable<CommentDTO>>.Error(new ErrorList(result.Errors));

        List<CommentDTO> comments = result.Value.Select(r => new CommentDTO {
            Id = r.Id,
            AuthorId = r.AuthorId,
            Content = r.Content,
            CreatedAt = r.CreatedAt,
            Type = Enum.GetName(r.Type)!,
            TargetId = r.TargetId
        }).ToList();

        return Result<IEnumerable<CommentDTO>>.Success(comments);
    }
}
