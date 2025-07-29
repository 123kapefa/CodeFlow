using Abstractions.Commands;

using Contracts.TagService;

namespace TagService.Application.Features.ParticipationTags.UpdateTags;

// TODO НЕ НУЖЕН !!! !!! УДАЛИТЬ
public record UpdateTagsCommand( UpdateParticipationDto UpdateDto) : ICommand;
