namespace Contracts.Requests.VoteService;

public record SetVoteRequest(Guid ParentId, Guid SourceId, Guid OwnerUserId, string SourceType, string ValueKind);
