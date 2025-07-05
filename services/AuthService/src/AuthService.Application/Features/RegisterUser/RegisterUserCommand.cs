using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;

namespace AuthService.Application.Features.RegisterUser;

public record RegisterUserCommand (string Email, string Password) : ICommand;