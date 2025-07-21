using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestionTags;
public record GetQuestionTagsCommand( Guid questionId ) : ICommand;
