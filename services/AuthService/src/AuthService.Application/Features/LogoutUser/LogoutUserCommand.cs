using Contracts.Commands;

namespace AuthService.Application.Features.LogoutUser;

public record LogoutUserCommand(string RefreshToken) : ICommand;