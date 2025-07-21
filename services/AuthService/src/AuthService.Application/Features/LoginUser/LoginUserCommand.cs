using Abstractions.Commands;

namespace AuthService.Application.Features.LoginUser;

public record LoginUserCommand (string Email, string Password) : ICommand;
