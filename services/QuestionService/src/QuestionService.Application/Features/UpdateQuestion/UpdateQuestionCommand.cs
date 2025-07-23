using Abstractions.Commands;

using Contracts.QuestionService.DTOs;

namespace QuestionService.Application.Features.UpdateQuestion;

public record UpdateQuestionCommand( UpdateQuestionDTO UpdateQuestionDTO ) : ICommand;
