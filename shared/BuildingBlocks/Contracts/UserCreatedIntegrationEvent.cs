namespace Contracts;

public record UserCreatedIntegrationEvent(
  Guid UserId,
  string Email,
  DateTime CreatedAt
);