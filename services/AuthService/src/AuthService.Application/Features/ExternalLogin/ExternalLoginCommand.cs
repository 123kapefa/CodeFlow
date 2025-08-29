using System.Security.Claims;

using Abstractions.Commands;

namespace AuthService.Application.Features.ExternalLogin;

public record ExternalLoginCommand (ClaimsPrincipal Principal) : ICommand;