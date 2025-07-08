using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security;

public class JwtTokenService : ITokenService {

  private readonly byte[] _key;
  private readonly JwtSettings _settings;

  public JwtTokenService (IOptions<JwtSettings> options) {
    _settings = options.Value;
    if (string.IsNullOrWhiteSpace(_settings.Secret))
      throw new InvalidOperationException("JWT Secret is not configured.");

    _key = Encoding.UTF8.GetBytes(_settings.Secret);
  }

  public (string AccessToken, int ExpiresInSeconds) GenerateTokens (Guid userId, string email) {
    var now = DateTime.UtcNow;
    var tokenDescriptor = new SecurityTokenDescriptor {
      Subject = new ClaimsIdentity (new[] {
          new Claim (JwtRegisteredClaimNames.Sub, userId.ToString ()), 
          new Claim (JwtRegisteredClaimNames.Email, email)
        }),
      Expires = now.AddSeconds (_settings.ExpiresInMinutes), 
      SigningCredentials = new SigningCredentials (
        new SymmetricSecurityKey (_key), 
        SecurityAlgorithms.HmacSha256Signature)
    };

    var handler = new JwtSecurityTokenHandler ();
    var secToken = handler.CreateToken (tokenDescriptor);
    var accessToken = handler.WriteToken (secToken);

    return (accessToken, _settings.ExpiresInMinutes);
  }

}