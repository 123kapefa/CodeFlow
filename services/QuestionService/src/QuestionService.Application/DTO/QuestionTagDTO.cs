using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.DTO;

public class QuestionTagDTO {
    public Guid Id { get; set; } = Guid.NewGuid(); // TODO пока пусть будет(на случай если будет нужен поиск по ID)
    public int TagId { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}
