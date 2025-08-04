using Abstractions.Commands;

using Contracts.Requests.AnswerService;

namespace AnswerService.Application.Features.EditAnswer;

public record EditAnswerCommand (Guid Id, UpdateAnswerRequest Request) : ICommand;