using Abstractions.Commands;

using ReputationService.Domain.Entities;

namespace ReputationService.Application.Features.AcceptedAnswerChanged;

public record AcceptedAnswerChangedCommand (
  Guid SourceEventId, string SourceService, string? CorrelationId,
  DateTime OccurredAt, Guid QuestionId,
  Guid? OldAnswerOwnerUserId, Guid? NewAnswerOwnerUserId, int Version) : ICommand;