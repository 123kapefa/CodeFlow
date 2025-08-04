namespace Contracts.DTOs.QuestionService;

public class QuestionShortDTO {
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public Guid? UserEditorId { get; set; }


    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int ViewsCount { get; set; } = 0;
    public int AnswersCount { get; set; } = 0;

    public int Upvotes { get; set; } = 0;
    public int Downvotes { get; set; } = 0;

    public bool IsClosed { get; set; } = false;
    public Guid? AcceptedAnswerId { get; set; }  
    
    public ICollection<QuestionTagDTO> QuestionTags { get; set; } = new List<QuestionTagDTO>();
}
