using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Domain.Entities;

public class QuestionChangingHistory {
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}