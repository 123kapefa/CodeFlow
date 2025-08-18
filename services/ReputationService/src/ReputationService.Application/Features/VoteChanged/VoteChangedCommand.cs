using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace ReputationService.Application.Features.VoteChanged;

public record VoteChangedCommand (
      Guid SourceEventId, string SourceService, string? CorrelationId,
      DateTime OccurredAt, VotableEntityType EntityType,
      Guid EntityId, Guid EntityOwnerUserId, Guid ActorUserId,
      VoteKind NewKind, int Version) : ICommand;