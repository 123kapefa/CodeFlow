using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.UpdateTags;

public class UpdateTagsHandler : ICommandHandler<UpdateTagsCommand> {

    private readonly IUserTagParticipationRepository _repository;

    public UpdateTagsHandler( IUserTagParticipationRepository repository ) {
        _repository = repository;
    }


    public async Task<Result> Handle( UpdateTagsCommand command, CancellationToken token ) {

        if(command.UpdateDto.UserId == Guid.Empty
            || command.UpdateDto.TagId <= 0)
            return Result.Error("Некорректный аргумент запроса");

        Result<UserTagParticipation?> resultTag = 
            await _repository.GetUserTagParticipation(command.UpdateDto.UserId, command.UpdateDto.TagId, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        if(command.UpdateDto.IsAnswer)
            resultTag.Value!.AnswersWritten += 1;
        else
            resultTag.Value!.QuestionsCreated += 1;

        //resultTag.Value.UserTagParticipationQuestions.Add(new UserTagParticipationQuestion {
        //    QuestionId = command.UpdateDto.QuestionId
        //});

        resultTag.Value.UserTagParticipationQuestions.Add(
               UserTagParticipationQuestion.Create(command.UpdateDto.QuestionId));
        resultTag.Value.LastActiveAt = DateTime.UtcNow;

        Result result = await _repository.UpdateAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
