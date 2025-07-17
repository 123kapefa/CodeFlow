using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Domain.Entities;

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Количество вопросов с этим тегом
    public int CountQuestion { get; set; }
    // Количество пиписчоков у тега
    public int CountWotchers { get; set; }
    // Количество вопросов с этим тегом за день
    public int DailyRequestCount { get; set; }
    // Количество вопросов с этим тегом за неделю
    public int WeeklyRequestCount { get; set; }

    // Навигация
    public ICollection<UserTagParticipation> UserTagParticipations { get; set; } = new List<UserTagParticipation>();
    public ICollection<WatchedTag> WatchedTags { get; set; } = new List<WatchedTag>();
}
