using Abstractions.Commands;

using Contracts.TagService;

namespace TagService.Application.Features.Tags.UpdateTag;

public record UpdateTagCommand(int tagId, TagUpdateDTO TagUpdateDTO) : ICommand;
