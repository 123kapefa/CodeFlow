using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

namespace TagService.Application.Features.ParticipationTags.CreateOrUpdateParticipationTags;

public record CreateOrUpdateParticipationTagsCommand (Guid QuestionId, Guid UserId, List<int> Tags) : ICommand;