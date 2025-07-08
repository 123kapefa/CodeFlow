using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Response;

public class UserInfoShortResponse {
    public string Username { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? AboutMe { get; set; }
    public string? AvatarUrl { get; set; }
    public int Reputation { get; set; }
    public List<string> Tags { get; set; } = [];
}
