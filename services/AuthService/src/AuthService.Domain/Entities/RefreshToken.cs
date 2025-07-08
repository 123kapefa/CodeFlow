namespace AuthService.Domain.Entities;

public class RefreshToken {

  public Guid Id { get; set; } = Guid.NewGuid ();
  public string Token { get; set; } = null!;
  public DateTime ExpiresAt { get; set; }
  public DateTime? RevokedAt { get; set; }
  public Guid UserId { get; set; }
  public UserData User { get; set; } = null!;

}