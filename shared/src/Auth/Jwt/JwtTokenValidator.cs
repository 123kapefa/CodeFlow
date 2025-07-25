using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace Auth.Jwt;

public class JwtTokenValidator {

  private readonly JwtOptions _options;

  public JwtTokenValidator (JwtOptions options) {
    _options = options ?? throw new ArgumentNullException(nameof(options));

  }

  public ClaimsPrincipal? Validate(string token) {
    var tokenHandler = new JwtSecurityTokenHandler();

    try {
      var validationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = _options.Issuer,
        ValidateAudience = true,
        ValidAudience = _options.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_options.Secret)),
        ClockSkew = TimeSpan.Zero
      };
      
      var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
      
      return principal;
    }
    catch (Exception) {
      return null;
    }
  }
}