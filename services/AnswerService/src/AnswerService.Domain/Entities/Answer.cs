namespace AnswerService.Domain.Entities;

public class Answer {
  
  public Guid Id { get; set; }
  public Guid QuestionId { get; set; }
  public Guid UserId { get; set; }
  public Guid? UserEditorId { get; set;}
  
  public string Content { get; set; } = String.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  
  public bool IsAccepted { get; set; }
  
  public int Upvotes { get; set; }
  public int Downvotes { get; set; }

  public IEnumerable<AnswerChangingHistory> AnswerChangingHistoriesChanges { get; set; } = new  List<AnswerChangingHistory> ();

}