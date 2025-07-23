namespace AnswerService.Domain.Entities;

public class AnswerChangingHistory {

  public Guid Id { get; set; } = Guid.NewGuid ();
	
  public Guid AnswerId { get; set; }
  public Guid UserId { get; set; }
	
  public string Content { get; set; } =  String.Empty;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  protected AnswerChangingHistory () { }

  private AnswerChangingHistory (Guid answerId, Guid userId, string content) {
	  AnswerId = answerId;
	  UserId = userId;
	  Content = content;
  }
  
  public static AnswerChangingHistory Create (Guid answerId, Guid userId, string content) => 
	  new AnswerChangingHistory(answerId, userId, content);
  
}