using Contracts.Publishers.VoteService;

namespace Contracts.Responses.VoteService;

public record VoteResponse (VoteKind OldKind, VoteKind NewKind);