using Abstractions.Commands;

namespace AuthService.Application.Features.ExternalLogin;

public record GithubLoginCommand(string Code) : ICommand;