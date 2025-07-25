namespace Auth.Jwt;

public class JwtToken {

  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
  public DateTime ExpiresAt { get; set; }

}