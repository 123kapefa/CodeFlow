using AnswerService.Application.DTOs;
using AnswerService.Application.Features.GetAnswersByQuestionId;

using Ardalis.Result;

namespace AnswerService.Application.Responses;

public record GetAnswersResponse (IEnumerable<AnswerDto> Answers);