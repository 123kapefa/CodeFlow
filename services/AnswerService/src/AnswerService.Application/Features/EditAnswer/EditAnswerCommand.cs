using AnswerService.Application.Requests;

using Contracts.Commands;

namespace AnswerService.Application.Features.EditAnswer;

public record EditAnswerCommand (Guid Id, UpdateAnswerRequest Request) : ICommand;