using Abstractions.Commands;

namespace TagService.Application.Features.ParticipationTags.DeleteUserTags;

public record DeleteUserTagsCommand(Guid UserId) : ICommand;
