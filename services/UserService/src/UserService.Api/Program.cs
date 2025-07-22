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

using UserService.Api.Extensions;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.UpdateUserReputation;
using UserService.Application.Features.UpdateUserVisit;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.CreateUserInfo;

var builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.UseCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

app.Run();