using Abstractions.Commands;

using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Repositories;
using UserService.Application.Features.UpdateUserInfo;
using Ardalis.Result;

using Contracts.UserService.DTOs;

using UserService.Application.Features.GetUsers;
using FluentValidation;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.UpdateUserReputation;
using UserService.Application.Features.UpdateUserVisit;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.CreateUserInfo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserServiceDbContext> (options =>
    options.UseNpgsql (builder.Configuration.GetConnectionString ("Main")));

builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();

builder.Services.AddScoped<ICommandHandler<UpdateUserInfoCommand>, UpdateUserInfoHandler>();
builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand>, GetUsersHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteUserCommand>, DeleteUserHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateUserReputationCommand>, UpdateUserReputationHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateUserVisitCommand>, UpdateUserVisitHandler>();
builder.Services.AddScoped<ICommandHandler<Result<UserFullInfoDTO>, GetUserFullInfoCommand>, GetUserFullInfoHandler>();
builder.Services.AddScoped<ICommandHandler<CreateUserInfoCommand>, CreateUserInfoHandler>();

builder.Services.AddScoped<IValidator<UpdateUserInfoCommand>, UpdateUserInfoValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "������ ������������ Swagger ��� UsertService"
    });

    options.EnableAnnotations();
});

builder.Services.AddControllers();

var app = builder.Build();

//TODO �������� ��� ��������� ��� (docker �������� � Production, � swagger ����������� �� Development)

//if (app.Environment.IsDevelopment ()) {
//    app.UseSwagger ();
//    app.UseSwaggerUI (options => {
//        options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
//    });
//}

app.UseSwagger ();
app.UseSwaggerUI (options => {
    options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
});
app.UseDeveloperExceptionPage ();

app.MapControllers ();

app.Run();