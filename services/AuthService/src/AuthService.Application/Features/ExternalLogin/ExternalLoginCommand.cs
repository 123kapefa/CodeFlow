using System.Security.Claims;

using Contracts.Commands;

namespace AuthService.Application.Features.ExternalLogin;

public record ExternalLoginCommand (ClaimsPrincipal Principal) : ICommand;