using Abstractions.Commands;

namespace UserService.Application.Features.CreateUserInfo;

public record CreateUserInfoCommand( Guid userId, string userName ) : ICommand;