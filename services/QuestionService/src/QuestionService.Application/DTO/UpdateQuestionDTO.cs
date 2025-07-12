using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.DTO;

public class UpdateQuestionDTO {
    public Guid Id { get; set; }
    public Guid UserEditorId { get; set; }    
    public string Content { get; set; } = string.Empty;
    public List<QuestionTagDTO> QuestionTagsDTO { get; set; } = [];
}
