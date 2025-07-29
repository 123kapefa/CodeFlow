namespace Contracts.TagService.Requests;

public class TagParticipationAnswerRequest {
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public List<AnswerTagDTO> Tags { get; set; } = new();
}
