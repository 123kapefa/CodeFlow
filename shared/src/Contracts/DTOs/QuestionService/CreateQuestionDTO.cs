namespace Contracts.DTOs.QuestionService;

public class CreateQuestionDTO {  
    public Guid UserId { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;    
    public List<CreateQuestionTagDTO> QuestionTagsDTO { get; set; } = [];
}
