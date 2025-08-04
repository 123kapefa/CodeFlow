using Abstractions.Commands;

namespace TagService.Application.Features.ParticipationTags.DeleteAnswerTags;

public record DeleteAnswerTagsCommand( Guid UserId, Guid QuestionId, IEnumerable<int> TagIds ) : ICommand;
