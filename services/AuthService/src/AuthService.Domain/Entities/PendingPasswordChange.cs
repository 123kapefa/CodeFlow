namespace AuthService.Domain.Entities;

public class PendingPasswordChange {

  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; }
  public string NewPassword { get; set; } = null!;
  public string Token { get; set; } = null!;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}