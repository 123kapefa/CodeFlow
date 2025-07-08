using Contracts.Commands;

namespace AuthService.Application.Features.RefreshToken;

public record RefreshTokenCommand (string RefreshToken) : ICommand;