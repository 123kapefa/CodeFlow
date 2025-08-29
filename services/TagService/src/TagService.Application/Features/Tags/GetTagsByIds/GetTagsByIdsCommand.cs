using Abstractions.Commands;

namespace TagService.Application.Features.Tags.GetTagsByIds;

public record GetTagsByIdsCommand (IEnumerable<int> TagIds) : ICommand;