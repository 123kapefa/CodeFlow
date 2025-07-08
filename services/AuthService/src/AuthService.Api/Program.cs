using AuthService.Api.Extensions;
using AuthService.Application.Features.LoginUser;
using AuthService.Application.Features.RegisterUser;
using AuthService.Domain.Entities;
using AuthService.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder (args);

builder.UseBase ();
builder.UseDatabase ();

builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator> ();
builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator> ();

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