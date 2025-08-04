using Abstractions.Commands;

using Contracts.DTOs.UserService;

namespace UserService.Application.Features.UpdateUserInfo;

public record UpdateUserInfoCommand( UserInfoUpdateDTO UserInfoUpdateDTO) : ICommand;