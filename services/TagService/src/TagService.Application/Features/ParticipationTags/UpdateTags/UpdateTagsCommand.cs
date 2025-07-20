using Contracts.Commands;
using TagService.Application.DTO;

namespace TagService.Application.Features.ParticipationTags.UpdateTags;

public record UpdateTagsCommand( UpdateParticipationDto UpdateDto) : ICommand;
