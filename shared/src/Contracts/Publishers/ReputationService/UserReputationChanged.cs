namespace Contracts.Publishers.ReputationService;

public sealed record UserReputationChanged(
  Guid UserId,
  int NewReputation,
  DateTime OccurredAt,
  string? CorrelationId = null
);