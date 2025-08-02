using Contracts.AnswerService.DTOs;

namespace Contracts.AnswerService.Responses;

public record GetAnswersResponse (IEnumerable<AnswerDto> Answers);