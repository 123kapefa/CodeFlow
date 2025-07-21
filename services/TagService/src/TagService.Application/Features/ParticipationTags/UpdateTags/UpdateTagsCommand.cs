using Abstractions.Commands;

using Contracts.TagService;

namespace TagService.Application.Features.ParticipationTags.UpdateTags;

public record UpdateTagsCommand( UpdateParticipationDto UpdateDto) : ICommand;
