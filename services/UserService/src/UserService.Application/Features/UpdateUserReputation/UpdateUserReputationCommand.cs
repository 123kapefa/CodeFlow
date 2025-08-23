using Abstractions.Commands;

namespace UserService.Application.Features.UpdateUserReputation;

public record UpdateUserReputationCommand (
  Guid UserId,
  int NewReputation,
  DateTime OccurredAt) : ICommand;