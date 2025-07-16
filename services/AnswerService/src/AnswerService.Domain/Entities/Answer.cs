namespace AnswerService.Domain.Entities;

public class Answer {
  
  public Guid Id { get; set; }
  public Guid QuestionId { get; set; }
  public Guid UserId { get; set; }
  public Guid? UserEditorId { get; set;}
  
  public string Content { get; set; } = String.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public bool IsAccepted { get; set; } = false;
  
  public int Upvotes { get; set; }
  public int Downvotes { get; set; }

  public List<AnswerChangingHistory> AnswerChangingHistoriesChanges { get; set; } = new  List<AnswerChangingHistory> ();

  protected Answer () { }

  private Answer (Guid questionId, Guid userId, string content) {
    Id = Guid.NewGuid();
    QuestionId = questionId;
    UserId = userId;
    Content = content;
  }

  public static Answer Create (Guid questionId, Guid userId, string content) =>
    new Answer(questionId, userId, content);
}