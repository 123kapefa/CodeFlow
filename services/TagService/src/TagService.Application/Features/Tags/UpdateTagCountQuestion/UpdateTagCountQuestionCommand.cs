using Abstractions.Commands;

using Contracts.DTOs.TagService;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;


public record UpdateTagCountQuestionCommand(IEnumerable<QuestionTagDTO> TagDTOs, Guid UserId, Guid QuestionId): ICommand;
