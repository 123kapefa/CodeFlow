using Abstractions.Commands;

namespace AnswerService.Application.Features.GetAnswerHistory;

public record GetAnswerHistoryCommand(Guid answerId) : ICommand;
