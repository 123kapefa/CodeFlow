namespace Contracts.DTOs.CommentService;

public class CreateCommentDTO {    
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = null!;
    public Guid TargetId { get; set; }
}
