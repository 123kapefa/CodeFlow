namespace Contracts.TagService.Requests;

public sealed class UpdateTagCountsRequest {
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public List<QuestionTagDTO> Tags { get; set; } = new();
}
