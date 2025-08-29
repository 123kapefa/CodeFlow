using Contracts.DTOs.QuestionService;

using QuestionService.Domain.Entities;

namespace QuestionService.Application.Extensions;

public static class QuestionExtensions {

  public static QuestionShortDTO ToQuestionShortDto (this Question question) =>
    new QuestionShortDTO {
      Id = question.Id
      , UserId = question.UserId
      , Title = question.Title
      , Content = question.Content
      , CreatedAt = question.CreatedAt
      , ViewsCount = question.ViewsCount
      , AnswersCount = question.AnswersCount
      , Upvotes = question.Upvotes
      , Downvotes = question.Downvotes
      , IsClosed = question.IsClosed
      , QuestionTags = question.QuestionTags.ToQuestionTagsDto ().ToList ()
    };
  
  public static IEnumerable<QuestionShortDTO> ToQuestionsShortDto (this IEnumerable<Question> questions)
    => questions
     .Select(i => new QuestionShortDTO {
        Id = i.Id,
        UserId = i.UserId,
        Title = i.Title,
        Content = i.Content,
        CreatedAt = i.CreatedAt,
        ViewsCount = i.ViewsCount,
        AnswersCount = i.AnswersCount,
        Upvotes = i.Upvotes,
        Downvotes = i.Downvotes,
        IsClosed = i.IsClosed,
        QuestionTags = i.QuestionTags.ToQuestionTagsDto ().ToList ()
      }).ToList();
  
  public static QuestionDTO ToQuestionDto (this Question question) => 
    new QuestionDTO { 
        Id = question.Id,
        UserId = question.UserId,
        UserEditorId = question.UserEditorId,
        Title = question.Title,
        Content = question.Content,
        CreatedAt = question.CreatedAt,
        UpdatedAt = question.UpdatedAt,
        ViewsCount = question.ViewsCount,
        AnswersCount = question.AnswersCount,
        Upvotes = question.Upvotes,
        Downvotes = question.Downvotes,
        IsClosed = question.IsClosed,
        AcceptedAnswerId = question.AcceptedAnswerId,
        QuestionChangingHistories = 
        question.QuestionChangingHistories
        .Select(qh => new QuestionHistoryDTO {
            UserId = qh.Id,
            Content = qh.Content,
            UpdatedAt = qh.UpdatedAt                
        })
        .ToList(),
        QuestionTags = 
        question.QuestionTags
        .Select(qt => new QuestionTagDTO {
            TagId = qt.TagId,
            WatchedAt = qt.WatchedAt
        })
        .ToList()
    };

}