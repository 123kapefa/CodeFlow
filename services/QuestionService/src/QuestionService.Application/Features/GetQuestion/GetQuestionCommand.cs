using System.Security.Claims;

using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestion;

public record GetQuestionCommand(Guid QuestionId) :ICommand;