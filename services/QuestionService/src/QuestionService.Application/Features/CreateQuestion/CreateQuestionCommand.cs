using Abstractions.Commands;

using Contracts.QuestionService.DTOs;

namespace QuestionService.Application.Features.CreateQuestion;

public record CreateQuestionCommand (CreateQuestionDTO CreateQuestionDTO) : ICommand;