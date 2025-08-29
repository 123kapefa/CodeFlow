using Abstractions.Commands;

using Contracts.DTOs.TagService;

namespace TagService.Application.Features.ParticipationTags.UpdateTags;

// TODO НЕ НУЖЕН !!! !!! УДАЛИТЬ
public record UpdateTagsCommand( UpdateParticipationDto UpdateDto) : ICommand;
