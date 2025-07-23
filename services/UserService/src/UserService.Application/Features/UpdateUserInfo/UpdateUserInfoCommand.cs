using Abstractions.Commands;

using Contracts.UserService.DTOs;

namespace UserService.Application.Features.UpdateUserInfo;

public record UpdateUserInfoCommand( UserInfoUpdateDTO UserInfoUpdateDTO) : ICommand;