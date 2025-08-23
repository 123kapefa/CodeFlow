using System.Text.Json;

using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using Abstractions.Commands;
using Messaging.Broker;
using Contracts.Publishers.QuestionService;
using Contracts.Publishers.VoteService;

namespace QuestionService.Application.Features.UpdateQuestionVote;

public class UpdateQuestionVoteHandler : ICommandHandler<UpdateQuestionVoteCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public UpdateQuestionVoteHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result> Handle( UpdateQuestionVoteCommand command, CancellationToken token ) {
        
        Console.WriteLine("UpdateQuestionVoteHandler Start working");
        Console.WriteLine($"Command: {JsonSerializer.Serialize(command)}");
        
        if(command.QuestionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");

        if(command.VoteValue == 0)
            return Result.Error("Допустимые значения: 1 или -1");

        Result<Question> questionResult =
            await _questionServiceRepository.GetQuestionShortAsync(command.QuestionId, token);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));
        
        switch (command.VoteValue) {
            case VoteKind.Up: {
                questionResult.Value.Upvotes += 1;
                break;
            }
            case VoteKind.Down: {
                questionResult.Value.Downvotes += 1;
                break;
            }
            case VoteKind.None: {
                break;
            }
        }

        Result updateResult =
            await _questionServiceRepository.UpdateQuestionAsync(questionResult.Value, token);

        if(!updateResult.IsSuccess)
            return Result.Error(new ErrorList(updateResult.Errors));

        await _questionServiceRepository.SaveChangesAsync(token);

        return Result.Success();
    }
}
