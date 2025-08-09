using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.UserService;
using Contracts.Responses.UserService;

using FluentValidation;

using UserService.Application.Features.CreateUserInfo;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.GetUsers;
using UserService.Application.Features.UpdateUserInfo;
using UserService.Application.Features.UpdateUserProfile;
using UserService.Application.Features.UpdateUserReputation;
using UserService.Application.Features.UpdateUserVisit;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Repositories;

namespace UserService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {
    
    builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();

    builder.Services.AddScoped<ICommandHandler<UpdateUserInfoCommand>, UpdateUserInfoHandler>();
    builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand>, GetUsersHandler>();
    builder.Services.AddScoped<ICommandHandler<DeleteUserCommand>, DeleteUserHandler>();
    builder.Services.AddScoped<ICommandHandler<UpdateUserReputationCommand>, UpdateUserReputationHandler>();
    builder.Services.AddScoped<ICommandHandler<UpdateUserVisitCommand>, UpdateUserVisitHandler>();
    builder.Services.AddScoped<ICommandHandler<UserFullInfoDTO, GetUserFullInfoCommand>, GetUserFullInfoHandler>();
    builder.Services.AddScoped<ICommandHandler<CreateUserInfoCommand>, CreateUserInfoHandler>();
    builder.Services.AddScoped<ICommandHandler<UpdateUserProfileResponse, UpdateUserProfileCommand>, UpdateUserProfileHandler>();

    builder.Services.AddScoped<IValidator<UpdateUserInfoCommand>, UpdateUserInfoValidator>();
    
    return builder;
  }

}