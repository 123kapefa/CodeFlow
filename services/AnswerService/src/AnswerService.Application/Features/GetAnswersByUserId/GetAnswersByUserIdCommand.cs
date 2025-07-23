using Abstractions.Commands;

namespace AnswerService.Application.Features.GetAnswersByUserId;

public record GetAnswersByUserIdCommand (Guid UserId) : ICommand;