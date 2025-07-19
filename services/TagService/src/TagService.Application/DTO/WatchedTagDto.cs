using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.DTO;

public class WatchedTagDTO {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }   
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
}
