using Abstractions.Commands;

namespace TagService.Application.Features.Tags.UpdateTagWatchers;

// TODO НЕ НУЖЕН !!! !!! УДАЛИТЬ
public record UpdateTagWatchersCommand(int TagId, int Count) : ICommand;
