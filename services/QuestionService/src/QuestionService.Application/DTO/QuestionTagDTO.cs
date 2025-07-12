using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.DTO;

public class QuestionTagDTO {    
    public int TagId { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;   
}
