using AuthService.Application.Abstractions;

namespace AuthService.Application.Features.Logout;

public record LogoutCommand(string RefreshToken) : ICommand;