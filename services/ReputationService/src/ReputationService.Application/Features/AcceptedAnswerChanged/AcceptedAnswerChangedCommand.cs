using Abstractions.Commands;

using ReputationService.Domain.Entities;

namespace ReputationService.Application.Features.AcceptedAnswerChanged;

public record AcceptedAnswerChangedCommand (
  Guid SourceEventId, 
  Guid ParentId,
  Guid? OldAnswerId,
  Guid? OldAnswerOwnerUserId,
  Guid NewAnswerId,
  Guid NewAnswerOwnerUserId,
  string SourceService,
  int Version,
  DateTime OccurredAt) : ICommand;
