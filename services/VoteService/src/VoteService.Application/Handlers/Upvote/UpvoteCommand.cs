using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace VoteService.Application.Handlers.Upvote;

public record UpvoteCommand (Guid OwnerUserId, Guid AuthorUserId, Guid SourceId, VotableSourceType SourceType, VoteKind VoteKind) : ICommand;