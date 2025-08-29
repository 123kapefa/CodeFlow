using Ardalis.Result;

using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using Messaging.Broker;

namespace QuestionService.Application.Features.UpdateQuestionAccept;

public class UpdateQuestionAcceptHandler : ICommandHandler<UpdateQuestionAcceptCommand> {

  private readonly IMessageBroker _messageBroker;
  private readonly IQuestionServiceRepository _questionServiceRepository;

  public UpdateQuestionAcceptHandler (
    IQuestionServiceRepository questionServiceRepository,
    IMessageBroker messageBroker) {
    _questionServiceRepository = questionServiceRepository;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (UpdateQuestionAcceptCommand command, CancellationToken cancellationToken) {
    if (command.QuestionId == Guid.Empty)
      return Result.Error ("ID вопроса не может быть пустым");

    if (command.AcceptedAnswerId == Guid.Empty)
      return Result.Error ("ID ответа не может быть пустым");

    Result<Question> questionResult =
      await _questionServiceRepository.GetQuestionShortAsync (command.QuestionId, cancellationToken);

    if (!questionResult.IsSuccess)
      return Result.Error (new ErrorList (questionResult.Errors));
    
      var oldAnswerId = questionResult.Value.AcceptedAnswerId;
      questionResult.Value.AcceptAnswer (command.AcceptedAnswerId);
      
      var updateResult = await _questionServiceRepository.UpdateQuestionAsync (questionResult.Value, cancellationToken);
      
      await _messageBroker.PublishAsync (
        new AnswerAccepted (
          EventId: Guid.NewGuid (), 
          ParentId: questionResult.Value.Id, 
          OldAnswerId: oldAnswerId ?? null,
          OldAnswerOwnerUserId: command.OldAcceptedAnswerId ?? null,
          NewAnswerId: (Guid)questionResult.Value.AcceptedAnswerId!,
          NewAnswerOwnerUserId: command.AnswerUserId, 
          Version: (int)questionResult.Value.AcceptedAnswerVersion!, 
          OccurredAt: DateTime.UtcNow), 
        cancellationToken);
      await _questionServiceRepository.SaveChangesAsync (cancellationToken);
    
    
    if (!updateResult.IsSuccess)
      return Result.Error (new ErrorList (updateResult.Errors));

    return Result.Success ();
  }

}