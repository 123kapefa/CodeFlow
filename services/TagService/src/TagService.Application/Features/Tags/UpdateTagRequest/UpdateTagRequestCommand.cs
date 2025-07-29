using Abstractions.Commands;

namespace TagService.Application.Features.Tags.UpdateTagRequest;

//TODO НЕ НУЖЕН !!!! !!! !!!
public record UpdateTagRequestCommand(string Name) : ICommand;
