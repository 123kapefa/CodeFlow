using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using FluentValidation;

using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.Publishers.QuestionService;
using Contracts.Responses.QuestionService;

using Messaging.Broker;

namespace QuestionService.Application.Features.CreateQuestion;

public class CreateQuestionHandler : ICommandHandler<CreatedQuestionResponse, CreateQuestionCommand> {

  private readonly IMessageBroker _messageBroker;
  private readonly IQuestionServiceRepository _questionServiceRepository;
  private readonly IValidator<CreateQuestionCommand> _validator;

  public CreateQuestionHandler (
    IQuestionServiceRepository questionServiceRepository,
    IValidator<CreateQuestionCommand> validator,
    IMessageBroker messageBroker) {
    _questionServiceRepository = questionServiceRepository;
    _validator = validator;
    _messageBroker = messageBroker;
  }

  public async Task<Result<CreatedQuestionResponse>> Handle (
    CreateQuestionCommand command,
    CancellationToken cancellationToken) {
    var validateResult = await _validator.ValidateAsync (command, cancellationToken);

    if (!validateResult.IsValid)
      return Result.Invalid (validateResult.AsErrors ());

    Question question = new Question {
      UserId = command.QuestionDto.UserId,
      Title = command.QuestionDto.Title,
      Content = command.QuestionDto.Content,
      CreatedAt = DateTime.UtcNow,
      QuestionChangingHistories =
        new List<QuestionChangingHistory> {
          new QuestionChangingHistory {
            UserId = command.QuestionDto.UserId, Content = command.QuestionDto.Content, UpdatedAt = DateTime.UtcNow
          }
        },
      QuestionTags = command.QuestionDto.NewTags
       .Select (q => new QuestionTag { TagId = (int)q.Id!, WatchedAt = DateTime.UtcNow }).ToList ()
    };

    Result result = await _questionServiceRepository.CreateQuestionAsync (question, cancellationToken);
    await _messageBroker.PublishAsync (
      new QuestionCreated (question.Id, question.UserId,
        command.QuestionDto.NewTags.Select (qtd => (int)qtd.Id!).ToList ()), cancellationToken);
    await _questionServiceRepository.SaveChangesAsync (cancellationToken);

    return result.IsSuccess
      ? Result<CreatedQuestionResponse>.Success (new CreatedQuestionResponse (question.Id))
      : Result<CreatedQuestionResponse>.Error (new ErrorList (result.Errors));
  }

}