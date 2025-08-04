using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

namespace QuestionService.Application.Features.UpdateQuestion;

public record UpdateQuestionCommand( UpdateQuestionDTO UpdateQuestionDTO ) : ICommand;
