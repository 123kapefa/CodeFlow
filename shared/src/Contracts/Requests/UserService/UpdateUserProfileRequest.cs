using Microsoft.AspNetCore.Http;

namespace Contracts.Requests.UserService;

public record UpdateUserProfileRequest (
  Guid UserId,
  string? Username,
  string? AboutMe,
  string? Location,
  string? GitHubUrl,
  string? WebsiteUrl,
  IFormFile? AvatarStream,
  bool RemoveAvatar = false);