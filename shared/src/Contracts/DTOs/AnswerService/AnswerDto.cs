namespace Contracts.AnswerService.DTOs;

public class AnswerDto {

  public Guid Id { get; set; }
  public Guid QuestionId { get; set; }
  public Guid UserId { get; set; }
  public Guid? UserEditorId { get; set; }

  public string Content { get; set; } = String.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime? EditedAt { get; set; }
  
  public int Upvotes { get; set; }
  public int Downvotes { get; set; }
  
  public bool IsAccepted { get; set; }
  
}