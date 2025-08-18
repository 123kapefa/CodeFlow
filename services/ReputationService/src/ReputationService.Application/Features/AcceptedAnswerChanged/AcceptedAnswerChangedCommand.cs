using Abstractions.Commands;

using ReputationService.Domain.Entities;

namespace ReputationService.Application.Features.AcceptedAnswerChanged;

public record AcceptedAnswerChangedCommand (
  Guid Userid,
  Guid SourceId,
  ReputationSourceType SourceType,
  ReasonCode ReasonCode,
  int Delta,
  DateTimeOffset OccurredAt,
  Guid SourceEventId,
  string SourceService,
  string? CorrelationId) : ICommand;