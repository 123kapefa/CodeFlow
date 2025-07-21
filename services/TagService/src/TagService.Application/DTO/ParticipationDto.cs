namespace TagService.Application.DTO;

public class ParticipationDTO {
    public Guid UserId { get; set; }
    public int TagId { get; set; }

    public string TagName { get; set; } = string.Empty;
    public DateTime LastActiveAt { get; set; }  

    public int QuestionsCreated { get; set; }  // количество вопросов с тегом    
    public int AnswersWritten { get; set; } // количество ответов с тегом


    protected ParticipationDTO() { }

    private ParticipationDTO(
        Guid userId, int tagId, DateTime lastActiveAt, int questionsCreated, int answersWritten, string tagName ) {
        UserId = userId;
        TagId = tagId;
        LastActiveAt = lastActiveAt;
        QuestionsCreated = questionsCreated;
        AnswersWritten = answersWritten;
        TagName = tagName;
    }

    public static ParticipationDTO Create( 
        Guid userId, int tagId, DateTime lastActiveAt, int questionsCreated, int answersWritten, string tagName ) {
        return new ParticipationDTO(userId, tagId, lastActiveAt, questionsCreated, answersWritten, tagName);
    }
}
