using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using Abstractions.Commands;
using Messaging.Broker;
using Contracts.Publishers.QuestionService;

namespace QuestionService.Application.Features.UpdateQuestionVote;

public class UpdateQuestionVoteHandler : ICommandHandler<UpdateQuestionVoteCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;
    private readonly IMessageBroker _messageBroker;

    public UpdateQuestionVoteHandler( IQuestionServiceRepository questionServiceRepository, IMessageBroker messageBroker ) {
        _questionServiceRepository = questionServiceRepository;
        _messageBroker = messageBroker;
    }

    public async Task<Result> Handle( UpdateQuestionVoteCommand command, CancellationToken token ) {
        if(command.QuestionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");

        if(command.VoteValue == 0)
            return Result.Error("Допустимые значения: 1 или -1");

        Result<Question> questionResult =
            await _questionServiceRepository.GetQuestionShortAsync(command.QuestionId, token);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        int value = 0;

        if(command.VoteValue == 1) {
            questionResult.Value.Upvotes += 1;
            value = 10;
        }
        else {
            questionResult.Value.Downvotes += 1; 
            value = -5;
        }

        Result updateResult =
            await _questionServiceRepository.UpdateQuestionAsync(questionResult.Value, token);

        if(!updateResult.IsSuccess)
            return Result.Error(new ErrorList(updateResult.Errors));

        // TODO отправка события тут
        await _messageBroker.PublishAsync(new QuestionVoted(questionResult.Value.UserId, value), token);
        await _questionServiceRepository.SaveChangesAsync(token);

        return Result.Success();
    }
}
