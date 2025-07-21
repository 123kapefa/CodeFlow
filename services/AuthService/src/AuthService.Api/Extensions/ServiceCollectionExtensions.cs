using Abstractions.Commands;

using AuthService.Application.Abstractions;
using AuthService.Application.Features.EditUser;
using AuthService.Application.Features.EmailChange;
using AuthService.Application.Features.EmailChangeConfirm;
using AuthService.Application.Features.LoginUser;
using AuthService.Application.Features.LogoutUser;
using AuthService.Application.Features.PasswordChange;
using AuthService.Application.Features.PasswordChangeConfirm;
using AuthService.Application.Features.RefreshToken;
using AuthService.Application.Features.RegisterUser;
using AuthService.Application.Features.RemoveUser;
using AuthService.Application.Features.RequestEmailChange;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Settings;

using Contracts.AuthService.Responses;

using FluentValidation;

namespace AuthService.Api.Extensions;

public static class ServiceCollectionExtensions {

  public static WebApplicationBuilder UseCustomServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<IPasswordChangeRepository, PasswordChangeRepository> ();
    
    
    builder.Services.Configure<JwtSettings>(
      builder.Configuration.GetSection("JwtSettings"));
    
    builder.Services.Configure<GoogleAuthSettings>(
      builder.Configuration.GetSection(GoogleAuthSettings.SectionName));
    
    builder.Services.AddSingleton<ITokenService, JwtTokenService>();
    builder.Services.AddScoped<IAuthTokenManager, AuthTokenManager>();
    
    
    builder.Services.AddScoped<ICommandHandler<Guid, RegisterUserCommand>, RegisterUserHandler> ();
    builder.Services.AddScoped<ICommandHandler<LoginResponse, LoginUserCommand>, LoginUserHandler> ();
    builder.Services.AddScoped<ICommandHandler<LogoutUserCommand>, LogoutUserHandler> ();
    builder.Services.AddScoped<ICommandHandler<EditUserDataResponse, EditUserCommand>, EditUserHandler> ();
    builder.Services.AddScoped<ICommandHandler<EmailChangeCommand>, EmailChangeHandler> ();
    builder.Services.AddScoped<ICommandHandler<EmailChangeConfirmCommand>, EmailChangeConfirmHandler> ();
    builder.Services.AddScoped<ICommandHandler<RemoveUserCommand>, RemoveUserHandler> ();
    
    builder.Services.AddScoped<ICommandHandler<PasswordChangeCommand>, PasswordChangeHandler> ();
    builder.Services.AddScoped<ICommandHandler<PasswordChangeConfirmCommand>, PasswordChangeConfirmHandler> ();
    
    builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator> ();
    builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator> ();
    builder.Services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenValidator> ();
    builder.Services.AddScoped<IValidator<EmailChangeCommand>, EmailChangeValidator> ();
    
    return builder;
  }

}