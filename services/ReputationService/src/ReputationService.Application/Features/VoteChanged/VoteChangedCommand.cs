using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace ReputationService.Application.Features.VoteChanged;

public record VoteChangedCommand (
  DateTime OccurredAt,
  VotableEntityType EntityType,
  Guid EntityId,
  Guid EntityOwnerUserId,
  Guid ActorUserId,
  VoteKind OldKind,
  VoteKind NewKind,
  Guid SourceEventId,
  string SourceService,
  string? CorrelationId = null) : ICommand;