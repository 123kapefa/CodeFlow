using Abstractions.Commands;

namespace AnswerService.Application.Features.DeleteAnswer;

public record DeleteAnswerCommand (Guid Id) : ICommand;