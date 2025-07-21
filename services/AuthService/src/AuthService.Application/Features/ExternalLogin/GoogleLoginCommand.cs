using Abstractions.Commands;

namespace AuthService.Application.Features.ExternalLogin;

public record GoogleLoginCommand (string IdToken) : ICommand;