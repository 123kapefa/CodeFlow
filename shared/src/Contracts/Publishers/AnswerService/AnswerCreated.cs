using Contracts.DTOs.TagService;

namespace Contracts.Publishers.AnswerService;

public record AnswerCreated (Guid QuestionId, Guid UserId, List<AnswerTagDTO> Tags);