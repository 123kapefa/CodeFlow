using Abstractions.Commands;

namespace TagService.Application.Features.Tags.UpdateTagWatchers;

public record UpdateTagWatchersCommand(int TagId, int Count) : ICommand;
