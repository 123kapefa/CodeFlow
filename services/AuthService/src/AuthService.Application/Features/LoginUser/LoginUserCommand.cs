using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;

namespace AuthService.Application.Features.LoginUser;

public record LoginUserCommand (string Email, string Password) : ICommand;
