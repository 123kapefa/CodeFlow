namespace Contracts.CommentService.DTOs;

public class CommentDTO {
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Type { get; set; } = null!;
    public Guid TargetId { get; set; }
}
