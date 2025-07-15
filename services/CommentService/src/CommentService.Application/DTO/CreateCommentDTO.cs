using CommentService.Domain.Enums;

namespace CommentService.Application.DTO;

public class CreateCommentDTO {    
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;   

    public TypeTarget Type { get; set; }
    public Guid TargetId { get; set; }
}
