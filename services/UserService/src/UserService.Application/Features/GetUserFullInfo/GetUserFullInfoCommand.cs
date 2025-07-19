using Contracts.Commands;

namespace UserService.Application.Features.GetUserFullInfo;

public record GetUserFullInfoCommand(Guid UserId) :ICommand;