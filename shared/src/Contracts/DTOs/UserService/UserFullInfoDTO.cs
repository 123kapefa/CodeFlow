namespace Contracts.DTOs.UserService;

public class UserFullInfoDTO {
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
    public string? Location { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? GitHubUrl { get; set; }

    public DateTime LastVisitAt { get; set; } = DateTime.UtcNow;
    public int VisitCount { get; set; } = 0;
    public int Reputation { get; set; } = 0;
}
