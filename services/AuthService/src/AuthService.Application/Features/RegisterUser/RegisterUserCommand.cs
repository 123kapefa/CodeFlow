using AuthService.Application.Abstractions;

namespace AuthService.Application.Features.Register;

public record RegisterUserCommand (string Email, string Password) : ICommand;