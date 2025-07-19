using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.DTO;

public class TagDTO {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }    
    public int CountQuestion { get; set; }    
    public int CountWotchers { get; set; }   
    public int DailyRequestCount { get; set; }    
    public int WeeklyRequestCount { get; set; }
}
