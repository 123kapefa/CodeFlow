using AuthService.Application.Abstractions;

namespace AuthService.Application.Features.RefreshToken;

public record RefreshTokenCommand (string RefreshToken) : ICommand;