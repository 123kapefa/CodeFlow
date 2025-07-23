using Abstractions.Commands;

using Contracts.TagService;

namespace TagService.Application.Features.ParticipationTags.CreateTags;

public record CreateTagsCommand (CreateParticipationDto CreateParticipationDto) : ICommand;