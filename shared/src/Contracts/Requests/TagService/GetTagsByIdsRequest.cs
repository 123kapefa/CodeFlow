namespace Contracts.Requests.TagService;

public record GetTagsByIdsRequest (IEnumerable<int> TagIds);