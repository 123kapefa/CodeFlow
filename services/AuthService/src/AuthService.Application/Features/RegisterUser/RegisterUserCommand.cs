using Contracts.Commands;

namespace AuthService.Application.Features.RegisterUser;

public record RegisterUserCommand (string Username, string Email, string Password) : ICommand;

