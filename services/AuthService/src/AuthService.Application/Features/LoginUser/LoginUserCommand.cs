using Contracts.Commands;

namespace AuthService.Application.Features.LoginUser;

public record LoginUserCommand (string Email, string Password) : ICommand;
