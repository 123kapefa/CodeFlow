namespace Contracts.QuestionService.DTOs;

public class QuestionHistoryDTO {
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
