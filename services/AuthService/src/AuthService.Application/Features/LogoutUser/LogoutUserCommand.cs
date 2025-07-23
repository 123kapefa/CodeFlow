using Abstractions.Commands;

namespace AuthService.Application.Features.LogoutUser;

public record LogoutUserCommand(string RefreshToken) : ICommand;