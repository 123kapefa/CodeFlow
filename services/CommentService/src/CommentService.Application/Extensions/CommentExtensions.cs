using CommentService.Domain.Entities;
using CommentService.Domain.Enums;

using Contracts.DTOs.CommentService;

namespace CommentService.Application.Extensions;

public static class CommentExtensions {

  public static CommentDTO ToCommentDto (this Comment comment) =>
    new CommentDTO {
      AuthorId = comment.AuthorId,
      Content = comment.Content,
      CreatedAt = comment.CreatedAt,
      Id = comment.Id,
      TargetId = comment.TargetId,
      Type = Enum.GetName (comment.Type) ?? throw new Exception()
    };

  public static IEnumerable<CommentDTO> ToCommentsDto (this IEnumerable<Comment> comments) {
    if (comments is null)
      return Enumerable.Empty<CommentDTO>();
    
    return comments.Select (comment => new CommentDTO {
      AuthorId = comment.AuthorId,
      Content = comment.Content,
      CreatedAt = comment.CreatedAt,
      Id = comment.Id,
      TargetId = comment.TargetId,
      Type = Enum.GetName (comment.Type) ?? throw new Exception ()
    });
  }
}