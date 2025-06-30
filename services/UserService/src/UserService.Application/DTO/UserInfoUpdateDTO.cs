using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTO;

public class UserInfoUpdateDTO {
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? AboutMe { get; set; }
    public string? Location { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? GitHubUrl { get; set; }
}
