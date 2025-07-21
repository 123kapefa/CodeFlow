using Contracts.Commands;
using TagService.Application.DTO;

namespace TagService.Application.Features.Tags.UpdateTag;

public record UpdateTagCommand(int tagId, TagUpdateDTO TagUpdateDTO) : ICommand;
