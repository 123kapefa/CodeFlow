using Abstractions.Commands;

namespace UserService.Application.Features.UpdateUserReputation;

public record UpdateUserReputationCommand (Guid UserId, int Reputation) : ICommand;