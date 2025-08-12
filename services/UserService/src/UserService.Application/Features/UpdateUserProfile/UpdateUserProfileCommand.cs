using Abstractions.Commands;

using Contracts.Requests.UserService;

using Microsoft.AspNetCore.Http;

namespace UserService.Application.Features.UpdateUserProfile;

public record UpdateUserProfileCommand (UpdateUserProfileRequest Request) : ICommand;