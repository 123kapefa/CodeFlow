using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Repositories;

public class ExternalTokenService : IExternalTokenService {
    private readonly IOptions<JwtSettings> _jwt;
    private readonly IUserDataRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;

    public ExternalTokenService( IOptions<JwtSettings> jwt, IUserDataRepository users, IRefreshTokenRepository refreshTokens ) {
        _jwt = jwt;
        _users = users;
        _refreshTokens = refreshTokens;
    }

    public async Task<(string Jwt, string Refresh)> IssueAsync( Guid userId, CancellationToken ct = default ) {
        var userRes = await _users.GetByIdAsync(userId);
        if(!userRes.IsSuccess) throw new InvalidOperationException("User not found");

        var user = userRes.Value!;
        var rolesRes = await _users.GetRolesAsync(user);
        var roles = rolesRes.IsSuccess ? rolesRes.Value! : Array.Empty<string>();

        var now = DateTime.UtcNow;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.Fullname ?? user.UserName ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var jwt = new JwtSecurityToken(
            issuer: _jwt.Value.Issuer,
            audience: _jwt.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_jwt.Value.ExpiresInMinutes),
            signingCredentials: creds);

        var jwtStr = new JwtSecurityTokenHandler().WriteToken(jwt);

        var rt = await _refreshTokens.CreateAsync(user.Id, TimeSpan.FromDays(30));
        if(!rt.IsSuccess) throw new InvalidOperationException("Cannot issue refresh token");

        return (jwtStr, rt.Value!.Token);
    }
}