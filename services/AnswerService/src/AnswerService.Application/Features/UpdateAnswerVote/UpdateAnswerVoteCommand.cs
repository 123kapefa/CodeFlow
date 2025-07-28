using Abstractions.Commands;

namespace AnswerService.Application.Features.UpdateAnswerVote;

public record UpdateAnswerVoteCommand (Guid AnswerId, int VoteValue) : ICommand;