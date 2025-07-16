

using AnswerService.Application.Requests;

using Contracts.Commands;

namespace AnswerService.Application.Features.CreateAnswer;

public record CreateAnswerCommand (CreateAnswerRequest Request) : ICommand;