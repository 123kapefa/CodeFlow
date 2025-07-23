using System.Xml.Linq;

namespace TagService.Domain.Entities;

public class UserTagParticipation {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }     
    
    public int QuestionsCreated { get; set; }  // количество вопросов с тегом    
    public int AnswersWritten { get; set; }  // количество ответов с тегом

    public DateTime LastActiveAt { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;

    public ICollection<UserTagParticipationQuestion> UserTagParticipationQuestions { get; set; } = new List<UserTagParticipationQuestion>();


    protected UserTagParticipation() { }

    private UserTagParticipation( Guid userId, int tagId, bool isAnswer ) {
        UserId = userId;
        TagId = tagId;
        LastActiveAt = DateTime.UtcNow;
        QuestionsCreated = isAnswer ? 0 : 1;
        AnswersWritten = isAnswer ? 1 : 0;
    }

    public static UserTagParticipation Create( Guid userId, int tagId, bool isAnswer ) {
        return new UserTagParticipation(userId, tagId, isAnswer);
    }
}
