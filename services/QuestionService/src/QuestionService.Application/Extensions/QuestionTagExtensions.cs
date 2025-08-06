using Contracts.DTOs.QuestionService;

using QuestionService.Domain.Entities;

namespace QuestionService.Application.Extensions;

public static class QuestionTagExtensions {

  public static QuestionTagDTO ToQuestionTagDto (this QuestionTag questionTag) =>
    new QuestionTagDTO { TagId = questionTag.TagId, WatchedAt = questionTag.WatchedAt };
  
  public static IEnumerable<QuestionTagDTO> ToQuestionTagsDto (this IEnumerable<QuestionTag> questionTags) =>
    questionTags.Select (qt => new QuestionTagDTO { TagId = qt.TagId, WatchedAt = qt.WatchedAt });

}