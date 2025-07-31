using Abstractions.Commands;

using Contracts.AnswerService.Requests;

namespace AnswerService.Application.Features.DeleteAnswer;

public record DeleteAnswerCommand (Guid Id, DeleteAnswerRequest Request) : ICommand;