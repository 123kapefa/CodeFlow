using Contracts.DTOs.QuestionService;
using Contracts.TagService;


namespace Contracts.Publishers.QuestionService;

public record QuestionCreated (Guid QuestionId, Guid UserId, List<int> Tags);