namespace Contracts.DTOs.UserService;

public record UserForQuestionDto (Guid UserId, string Username, int Reputation, string? AvatarUrl);