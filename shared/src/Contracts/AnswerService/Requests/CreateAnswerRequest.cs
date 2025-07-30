using Contracts.TagService;

namespace Contracts.AnswerService.Requests;

public record CreateAnswerRequest (Guid QuestionId, Guid UserId, string Content, List<AnswerTagDTO> Tags);