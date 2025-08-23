using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Domain.Entities;

public class Question {

  public Guid Id { get; set; } = Guid.NewGuid ();

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
  public int? AcceptedAnswerVersion { get; private set; }

  public List<QuestionChangingHistory> QuestionChangingHistories { get; set; } = [];
  public List<QuestionTag> QuestionTags { get; set; } = [];

  public void AcceptAnswer (Guid? newAnswerId) {
    AcceptedAnswerVersion ??= 1;

    if (AcceptedAnswerId == newAnswerId) return;
    
    if (AcceptedAnswerId is null) {
      IsClosed = false;
      AcceptedAnswerId = null;
    }
    
    IsClosed = true;
    AcceptedAnswerId = newAnswerId;
    AcceptedAnswerVersion++;
  }

}