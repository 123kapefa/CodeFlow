using Abstractions.Commands;

using Contracts.Requests.AnswerService;

namespace AnswerService.Application.Features.DeleteAnswer;

public record DeleteAnswerCommand (Guid Id, DeleteAnswerRequest Request) : ICommand;