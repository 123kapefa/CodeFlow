using Contracts.Commands;

namespace TagService.Application.Features.Tags.UpdateTagRequest;

//TODO ПОДУМАТЬ НА СЧЕТ ПАРАМЕТРОВ ... МОЖЕТ int tagID???? ... МОЖЕТ List<string> names???
public record UpdateTagRequestCommand(string Name) : ICommand;
