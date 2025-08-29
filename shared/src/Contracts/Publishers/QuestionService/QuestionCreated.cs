using Contracts.DTOs.QuestionService;


namespace Contracts.Publishers.QuestionService;

public record QuestionCreated (Guid QuestionId, Guid UserId, List<int> Tags);