using Contracts.Commands;

namespace UserService.Application.Features.UpdateUserVisit;

public record UpdateUserVisitCommand(Guid UserId) : ICommand;