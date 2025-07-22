using Ardalis.Result;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestionService.Application.Features.CreateQuestion;
using QuestionService.Application.Features.DeleteQuestion;
using QuestionService.Application.Features.GetQuestion;
using QuestionService.Application.Features.GetQuestionHistory;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Application.Features.GetQuestionTags;
using QuestionService.Application.Features.GetUserQuestions;
using QuestionService.Application.Features.UpdateQuestion;
using QuestionService.Application.Features.UpdateQuestionAccept;
using QuestionService.Application.Features.UpdateQuestionAnswers;
using QuestionService.Application.Features.UpdateQuestionView;
using QuestionService.Application.Features.UpdateQuestionVote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Reflection;

using Abstractions.Commands;

using Contracts.QuestionService.DTOs;

using QuestionService.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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