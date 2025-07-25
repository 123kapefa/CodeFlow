using Contracts.QuestionService.DTOs;
using QuestionService.Domain.Entities;

namespace QuestionService.Application;

public static class QuestionExtension {


    public static Question FromDto( this CreateQuestionDTO questionDTO ) {

        Question question = new Question {
            UserId = questionDTO.UserId,
            Title = questionDTO.Title,
            Content = questionDTO.Content,
            CreatedAt = DateTime.UtcNow,

            QuestionTags = questionDTO.QuestionTagsDTO
            .Select(qt => new QuestionTag { TagId = qt.TagId, WatchedAt = DateTime.UtcNow }).ToList(),

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
