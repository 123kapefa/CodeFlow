using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security;

public class JwtTokenService : ITokenService {

  private readonly byte[] _key;
  private readonly int _expiresIn;

  public JwtTokenService (IConfiguration cfg) {
    // Настройте в appsettings.json:
    // "Jwt": { "Secret": "...", "ExpiresInSeconds": 3600 }
    _key = Encoding.UTF8.GetBytes (cfg["Jwt:Secret"]!);
    _expiresIn = int.Parse (cfg["Jwt:ExpiresInSeconds"]!);
  }

  public (string AccessToken, string RefreshToken, int ExpiresInSeconds) GenerateTokens (Guid userId, string email) {
    var now = DateTime.UtcNow;
    var tokenDescriptor = new SecurityTokenDescriptor {
      Subject = new ClaimsIdentity (new[] {
          new Claim (JwtRegisteredClaimNames.Sub, userId.ToString ()), 
          new Claim (JwtRegisteredClaimNames.Email, email)
        }),
      Expires = now.AddSeconds (_expiresIn), 
      SigningCredentials = new SigningCredentials (
        new SymmetricSecurityKey (_key), 
        SecurityAlgorithms.HmacSha256Signature)
    };

    var handler = new JwtSecurityTokenHandler ();
    var secToken = handler.CreateToken (tokenDescriptor);
    var accessToken = handler.WriteToken (secToken);

    // Для простоты: refresh-токен — GUID строкой
    var refreshToken = Guid.NewGuid ().ToString ();

    return (accessToken, refreshToken, _expiresIn);
  }

}