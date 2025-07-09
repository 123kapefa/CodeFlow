namespace UserService.Application.DTO;

public class UserShortDTO {
    public string UserName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? AboutMe { get; set; }
    public string? AvatarUrl { get; set; }
    public int Reputation { get; set; }
    public List<string> Tags { get; set; } = [];
}