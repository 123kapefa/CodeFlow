using Abstractions.Commands;

using Contracts.Requests.AnswerService;

namespace AnswerService.Application.Features.CreateAnswer;

public record CreateAnswerCommand (CreateAnswerRequest Request) : ICommand;