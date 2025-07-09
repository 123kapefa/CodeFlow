using Contracts.Commands;
using UserService.Application.DTO;

namespace UserService.Application.Features.UpdateUserInfo;

public record UpdateUserInfoCommand( UserInfoUpdateDTO UserInfoUpdateDTO) : ICommand;