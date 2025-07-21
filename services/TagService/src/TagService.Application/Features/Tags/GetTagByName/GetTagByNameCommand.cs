using Contracts.Commands;

namespace TagService.Application.Features.Tags.GetTagByName;

public record GetTagByNameCommand(string Name) : ICommand;
