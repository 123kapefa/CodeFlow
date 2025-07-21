namespace TagService.Domain.Entities;

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
   
    public int CountQuestion { get; set; } //обновляется при создании вопроса  
    public int CountWotchers { get; set; } //обновляется при добавлении в отслеживаемые в IWatchedTagRepository

    public int DailyRequestCount { get; set; } //обновляется при создании вопроса
    public int WeeklyRequestCount { get; set; } //обновляется при создании вопроса

    public DateTime? DailyCountUpdatedAt { get; set; } //когда обновляли «сутки»
    public DateTime? WeeklyCountUpdatedAt { get; set; } //когда обновляли «неделю»

    // Навигация
    public ICollection<UserTagParticipation> UserTagParticipations { get; set; } = new List<UserTagParticipation>();
    public ICollection<WatchedTag> WatchedTags { get; set; } = new List<WatchedTag>();



    protected Tag() { }

    private Tag( string name, string? description ) {
        Name = name;
        Description = description;
        CreatedAt  = DateTime.UtcNow;
    }

    public static Tag Create( string name, string? description ) {
        return new Tag(name, description);
    }
}
