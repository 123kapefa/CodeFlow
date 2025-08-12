using Abstractions.Commands;

using Ardalis.Result;

using CommentService.Application.Extensions;
using CommentService.Domain.Repositories;

using Contracts.DTOs.CommentService;

namespace CommentService.Application.Features.GetCommentsByAnswerIds;

public class GetCommentsByAnswerIdsHandler : ICommandHandler<IEnumerable<CommentDTO>, GetCommentsByAnswerIdsCommand> {

  private readonly ICommentRepository _commentRepository;
  
  public GetCommentsByAnswerIdsHandler (ICommentRepository commentRepository) {
    _commentRepository = commentRepository;
  }

  public async Task<Result<IEnumerable<CommentDTO>>> Handle (GetCommentsByAnswerIdsCommand command, CancellationToken cancellationToken) {
    var result = await _commentRepository.GetCommentsByAnswerIdsAsync(command.AnswerIds, cancellationToken);

    if (!result.IsSuccess) {
      return Result<IEnumerable<CommentDTO>>.Error(new ErrorList(result.Errors));
    }

    return Result<IEnumerable<CommentDTO>>.Success(result.Value.ToCommentsDto ());
  }

}