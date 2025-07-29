using Abstractions.Commands;
using Contracts.TagService;

namespace TagService.Application.Features.ParticipationTags.UpdateParticipationAnswer;

public record UpdateParticipationAnswerCommand( 
    IEnumerable<AnswerTagDTO> TagDTOs, Guid UserId, Guid QuestionId ) : ICommand;
