namespace Contracts.DTOs.AnswerService;

public record AnswerHistoryDTO ( Guid AnswerId, Guid UserId, string Content, DateTime UpdatedAt );
