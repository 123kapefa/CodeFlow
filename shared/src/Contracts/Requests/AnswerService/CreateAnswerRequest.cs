using Contracts.DTOs.TagService;

namespace Contracts.Requests.AnswerService;

public record CreateAnswerRequest (Guid QuestionId, Guid UserId, string Content, List<AnswerTagDTO> Tags);