using Abstractions.Commands;
using Contracts.TagService;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;


public record UpdateTagCountQuestionCommand(IEnumerable<QuestionTagDTO> TagDTOs, Guid UserId, Guid QuestionId): ICommand;
