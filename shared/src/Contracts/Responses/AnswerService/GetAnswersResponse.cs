using Contracts.DTOs.AnswerService;

namespace Contracts.Responses.AnswerService;

public record GetAnswersResponse (IEnumerable<AnswerDto> Answers);