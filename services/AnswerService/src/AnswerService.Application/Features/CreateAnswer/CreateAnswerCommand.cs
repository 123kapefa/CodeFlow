using Abstractions.Commands;

using Contracts.AnswerService.Requests;

namespace AnswerService.Application.Features.CreateAnswer;

public record CreateAnswerCommand (CreateAnswerRequest Request) : ICommand;