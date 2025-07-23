using Abstractions.Commands;

using Contracts.AnswerService.Requests;

namespace AnswerService.Application.Features.EditAnswer;

public record EditAnswerCommand (Guid Id, UpdateAnswerRequest Request) : ICommand;