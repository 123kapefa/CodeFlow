namespace TagService.Application.DTO;

public class ParticipationDTO {
    public Guid UserId { get; set; }
    public int TagId { get; set; }

    public string TagName { get; set; } = string.Empty;
    public DateTime LastActiveAt { get; set; }  

    public int QuestionsCreated { get; set; }  // количество вопросов с тегом    
    public int AnswersWritten { get; set; } // количество ответов с тегом
}
