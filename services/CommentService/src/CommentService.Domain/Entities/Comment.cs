using CommentService.Domain.Enums;

namespace CommentService.Domain.Entities;

public class Comment {    
    public Guid Id { get; set; }    
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public TypeTarget Type { get; set; }
    public Guid TargetId { get; set; }     
}
