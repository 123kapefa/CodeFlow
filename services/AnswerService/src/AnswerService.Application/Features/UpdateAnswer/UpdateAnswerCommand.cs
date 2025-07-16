using AnswerService.Application.Requests;

using Contracts.Commands;

namespace AnswerService.Application.Features.UpdateAnswer;

public record UpdateAnswerCommand (Guid Id, UpdateAnswerRequest Request) : ICommand;