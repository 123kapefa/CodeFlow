using Abstractions.Commands;

using Contracts.Publishers.VoteService;

using VoteService.Domain.Entities;

namespace VoteService.Application.Handlers.SetVote;

public record SetVoteCommand (
  Guid ParentId, 
  Guid SourceId, 
  Guid OwnerUserId, 
  Guid AuthorUserId, 
  VotableSourceType SourceType, 
  VoteKind VoteKind) : ICommand;