using Contracts.Commands;

namespace AnswerService.Application.Features.DeleteAnswer;

public record DeleteAnswerCommand (Guid Id) : ICommand;