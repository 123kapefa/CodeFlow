namespace Contracts.DTOs.TagService;

public class TagDTO {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }    
    public int CountQuestion { get; set; }    
    public int CountWotchers { get; set; }   
    public int DailyRequestCount { get; set; }    
    public int WeeklyRequestCount { get; set; }

    public TagDTO() { }

    private TagDTO(
        int tagId, 
        string tagName, 
        string? description, 
        DateTime createdAt, 
        int countQuestion, 
        int countWotchers, 
        int dailyRequestCount, 
        int weeklyRequestCount ) {
        Id = tagId;
        Name = tagName;
        Description = description; 
        CreatedAt = createdAt;
        CountQuestion = countQuestion;
        CountWotchers = countWotchers;
        DailyRequestCount = dailyRequestCount;
        WeeklyRequestCount = weeklyRequestCount;
    }

    public static TagDTO Create( 
        int tagId,
        string tagName,
        string? description,
        DateTime createdAt,
        int countQuestion,
        int countWotchers,
        int dailyRequestCount,
        int weeklyRequestCount ) =>
        new TagDTO(tagId, tagName, description, createdAt, countQuestion, countWotchers, dailyRequestCount, weeklyRequestCount);
}
