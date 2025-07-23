using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using Abstractions.Commands;

namespace QuestionService.Application.Features.CreateQuestion;

public class CreateQuestionHandler : ICommandHandler<CreateQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;
    private readonly IValidator<CreateQuestionCommand> _validator;

    public CreateQuestionHandler( IQuestionServiceRepository questionServiceRepository, IValidator<CreateQuestionCommand> validator ) {
        _questionServiceRepository = questionServiceRepository;
        _validator = validator;
    }

    public async Task<Result> Handle( CreateQuestionCommand command, CancellationToken cancellationToken ) {

        var validateResult = await _validator.ValidateAsync(command);

        if(!validateResult.IsValid)
            return Result.Invalid(validateResult.AsErrors());

        Question question = new Question {
            UserId = command.CreateQuestionDTO.UserId,
            Title = command.CreateQuestionDTO.Title,
            Content = command.CreateQuestionDTO.Content,
            CreatedAt = DateTime.UtcNow,
            QuestionChangingHistories = 
            new List<QuestionChangingHistory> { new QuestionChangingHistory {
                UserId = command.CreateQuestionDTO.UserId,
                Content = command.CreateQuestionDTO.Content,
                UpdatedAt = DateTime.UtcNow
            } },
            QuestionTags = command.CreateQuestionDTO.QuestionTagsDTO.Select(q => new QuestionTag {
                TagId = q.TagId,
                WatchedAt = DateTime.UtcNow  
            }).ToList()
        };

        Result result = await _questionServiceRepository.CreateQuestionAsync(question, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
