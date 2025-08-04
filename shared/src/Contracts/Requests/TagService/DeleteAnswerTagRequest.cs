namespace Contracts.Requests.TagService;

public class DeleteAnswerTagRequest {
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public IEnumerable<int> TagIds { get; set; }
}
