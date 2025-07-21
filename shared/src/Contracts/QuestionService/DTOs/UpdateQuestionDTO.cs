namespace Contracts.QuestionService.DTOs;

public class UpdateQuestionDTO {
    public Guid Id { get; set; }
    public Guid UserEditorId { get; set; }    
    public string Content { get; set; } = string.Empty;
    public List<QuestionTagDTO> QuestionTagsDTO { get; set; } = [];
}
