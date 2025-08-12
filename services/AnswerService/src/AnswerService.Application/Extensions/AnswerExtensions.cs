
using AnswerService.Domain.Entities;

using Contracts.DTOs.AnswerService;

namespace AnswerService.Application.Extensions;

public static class AnswerExtensions {

  public static IEnumerable<AnswerDto> ToAnswersDto (this IEnumerable<Answer> answers) {
    
    List<AnswerDto> dto = new List<AnswerDto>();
    
    foreach (var answer in answers) {
      dto.Add(new AnswerDto() {
        Id = answer.Id,
        QuestionId = answer.QuestionId,
        UserId = answer.UserId,
        UserEditorId = answer.UserEditorId,
        Content = answer.Content,
        CreatedAt = answer.CreatedAt,
        EditedAt = answer.UpdatedAt,
        Upvotes = answer.Upvotes,
        Downvotes = answer.Downvotes,
      });
    }
    
    return dto;
  }

}