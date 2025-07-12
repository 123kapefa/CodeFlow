using QuestionService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.DTO;

public class CreateQuestionDTO {  
    public Guid UserId { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;    
    public List<QuestionTagDTO> QuestionTagsDTO { get; set; } = [];
}
