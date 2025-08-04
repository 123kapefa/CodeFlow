namespace Contracts.DTOs.QuestionService;

public class QuestionTagDTO {    
    public int TagId { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;   
}
