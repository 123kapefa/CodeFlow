using CommentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Domain.Entities;

public class Comment {    
    public Guid Id { get; set; }    
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public TypeTarget Type { get; set; }
    public Guid TargetId { get; set; }     
}
