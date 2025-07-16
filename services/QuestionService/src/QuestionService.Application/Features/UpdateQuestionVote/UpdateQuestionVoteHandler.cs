using Ardalis.Result;
using Contracts.Commands;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.UpdateQuestionVote;

public class UpdateQuestionVoteHandler : ICommandHandler<UpdateQuestionVoteCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public UpdateQuestionVoteHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result> Handle( UpdateQuestionVoteCommand command, CancellationToken cancellationToken ) {
        if(command.QuestionId == Guid.Empty)
            return Result.Error("ID вопроса не может быть пустым");

        if(command.VoteValue == 0)
            return Result.Error("Допустимые значения: 1 или -1");

        Result<Question> questionResult =
            await _questionServiceRepository.GetQuestionShortAsync(command.QuestionId, cancellationToken);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        if(command.VoteValue == 1)
            questionResult.Value.Upvotes += 1;
        else
            questionResult.Value.Downvotes += 1;

        Result updateResult =
            await _questionServiceRepository.UpdateQuestionAsync(questionResult.Value, cancellationToken);

        if(!updateResult.IsSuccess)
            return Result.Error(new ErrorList(updateResult.Errors));

        return Result.Success();
    }
}
