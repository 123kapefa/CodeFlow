namespace Contracts.Publishers.ReputationService;

public sealed record UserReputationChanged(
  Guid UserId,
  int NewReputation,
  DateTimeOffset OccurredAt
);