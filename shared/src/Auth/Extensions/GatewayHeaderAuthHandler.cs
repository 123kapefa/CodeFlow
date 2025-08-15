using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Authorization;

public sealed class GatewayHeaderAuthHandler
  : AuthenticationHandler<AuthenticationSchemeOptions> {
    public const string SchemeName = "Gateway";

    public GatewayHeaderAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock )
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var userId = Request.Headers["X-User-Id"].FirstOrDefault();
        if(string.IsNullOrWhiteSpace(userId))
            return Task.FromResult(AuthenticateResult.NoResult()); // аноним

        var claims = new List<Claim> { new("sub", userId) };

        var name = Request.Headers["X-User-Name"].FirstOrDefault();
        if(!string.IsNullOrWhiteSpace(name))
            claims.Add(new Claim("name", name));

        var roles = Request.Headers["X-User-Roles"].FirstOrDefault();
        if(!string.IsNullOrWhiteSpace(roles))
            claims.AddRange(
              roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .Select(r => new Claim("role", r)));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
