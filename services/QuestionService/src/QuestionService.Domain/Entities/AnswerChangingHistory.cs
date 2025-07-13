namespace QuestionService.Domain.Entities;

public class AnswerChangingHistory {

  public Guid Id { get; set; }
	
  public Guid AnswerId { get; set; }
  public Guid UserId { get; set; }
	
  public string Content { get; set; } =  String.Empty;
  public DateTime UpdatedAt { get; set; }

}