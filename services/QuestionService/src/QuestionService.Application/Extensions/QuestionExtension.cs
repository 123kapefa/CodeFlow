using Contracts.DTOs.QuestionService;

using QuestionService.Domain.Entities;

namespace QuestionService.Application;

public static class QuestionExtension {


    public static Question FromDto( this CreateQuestionDTO questionDTO ) {

        Question question = new Question {
            UserId = questionDTO.UserId,
            Title = questionDTO.Title,
            Content = questionDTO.Content,
            CreatedAt = DateTime.UtcNow,

            QuestionTags = questionDTO.NewTags
            .Select(qt => new QuestionTag { TagId = (int)qt.Id!, WatchedAt = DateTime.UtcNow }).ToList(),

            QuestionChangingHistories = new List<QuestionChangingHistory> {
                new QuestionChangingHistory {
                    UserId = questionDTO.UserId,
                    Content = questionDTO.Content,
                    UpdatedAt = DateTime.UtcNow }
            }
        };

        return question;
    }

}
