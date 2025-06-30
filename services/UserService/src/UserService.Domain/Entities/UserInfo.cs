namespace UserService.Domain.Entities;

public class UserInfo {
    public Guid Id { get; set; } = Guid.NewGuid ();

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
    public string? Location { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? GitHubUrl { get; set; }
}
