using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace AnswerService.Application.Features.UpdateAnswerVote;

public record UpdateAnswerVoteCommand (
  Guid AnswerId, 
  Guid AuthorUserId,
  VoteKind VoteValue) : ICommand;
