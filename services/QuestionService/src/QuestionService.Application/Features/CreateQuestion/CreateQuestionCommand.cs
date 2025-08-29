using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

namespace QuestionService.Application.Features.CreateQuestion;

public record CreateQuestionCommand (CreateQuestionDTO QuestionDto) : ICommand;