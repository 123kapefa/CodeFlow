using Abstractions.Commands;

using Contracts.DTOs.TagService;

namespace TagService.Application.Features.ParticipationTags.UpdateParticipationAnswer;

public record UpdateParticipationAnswerCommand( 
    IEnumerable<AnswerTagDTO> TagDTOs, Guid UserId, Guid QuestionId ) : ICommand;
