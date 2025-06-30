using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTO;

public class UserStatisticUpdateDto {
    public Guid UserId { get; set; }
    public DateTime? LastVisitAt { get; set; }
    public int VisitCount { get; set; }
    public int Reputation { get; set; }
}
