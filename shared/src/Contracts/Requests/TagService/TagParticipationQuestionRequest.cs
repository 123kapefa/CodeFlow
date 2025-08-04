using Contracts.DTOs.TagService;

namespace Contracts.Requests.TagService;

public sealed class TagParticipationQuestionRequest {
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public List<QuestionTagDTO> Tags { get; set; } = new();
}
