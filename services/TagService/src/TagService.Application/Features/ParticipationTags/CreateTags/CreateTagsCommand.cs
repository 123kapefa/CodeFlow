using Contracts.Commands;
using TagService.Application.DTO;

namespace TagService.Application.Features.ParticipationTags.CreateTags;

public record CreateTagsCommand (CreateParticipationDto CreateParticipationDto) : ICommand;