using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestionTitlesByIds;

public record GetQuestionTitlesByIdsCommand (IEnumerable<Guid> QuestionIds) : ICommand;