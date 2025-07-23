using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Domain.Entities;

public class QuestionTag {
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public int TagId { get; set; }

    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}