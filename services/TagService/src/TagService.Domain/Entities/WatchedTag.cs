using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Domain.Entities;

public class WatchedTag {
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; } 
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
