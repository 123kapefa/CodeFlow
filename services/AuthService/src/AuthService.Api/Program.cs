using System.Security.Claims;

using AuthService.Api.Extensions;
using AuthService.Application.Abstractions;
using AuthService.Application.Features.LoginUser;
using AuthService.Application.Features.RegisterUser;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Email;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Settings;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder (args);

var config = new ConfigurationBuilder()
 .AddUserSecrets<Program>()
 .Build();

builder.UseBase ();
builder.UseDatabase ();

builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator> ();
builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator> ();

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender> ();


builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<UserData>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
   .AddRoles<IdentityRole<Guid>>()
   .AddEntityFrameworkStores<AuthServiceDbContext>()
   .AddSignInManager<SignInManager<UserData>>()
   .AddDefaultTokenProviders();


builder.UseCustomServices ();
builder.UseCustomSerilog ();
builder.UseMail ();
builder.Services.AddEndpointsApiExplorer ();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p => p
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());
});

var app = builder.Build ();

app.UseCors("AllowAll");
app.UseSwagger ();
app.UseSwaggerUI (options => {
    options.SwaggerEndpoint ("/swagger/v1/swagger.json", "AuthService API v1");
});

app.UseHttpsRedirection ();

app.UseAuthorization ();
app.MapControllers ();

app.Run ();