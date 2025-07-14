using CommentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.DTO;

public class CreateCommentDTO {    
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;   

    public TypeTarget Type { get; set; }
    public Guid TargetId { get; set; }
}
