using Abstractions.Commands;

namespace AnswerService.Application.Features.DeleteAnswersByUserId;

public record DeleteAnswersByUserIdCommand (Guid UserId) : ICommand;