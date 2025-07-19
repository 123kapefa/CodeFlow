using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.DTO;
internal class WatchedTagCreateDTO {
    public Guid UserId { get; set; }    
    public int TagId { get; set; }
}
