using Abstractions.Commands;

using Contracts.TagService;

namespace TagService.Application.Features.ParticipationTags.CreateTags;

// TODO НЕ НУЖЕН !!! !!! УДАЛИТЬ
public record CreateTagsCommand (CreateParticipationDto CreateParticipationDto) : ICommand;