using Contracts.Publishers.VoteService;

namespace Contracts.Requests.VoteService;

public record UpvoteRequest (Guid OwnerUserId, Guid SourceId, string SourceType);