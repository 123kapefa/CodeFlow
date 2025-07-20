using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.CreateTags;

public class CreateTagsHandler : ICommandHandler<CreateTagsCommand> {

    private readonly IUserTagParticipationRepository _repository;

    public CreateTagsHandler( IUserTagParticipationRepository userTagParticipationRepository ) {
        _repository = userTagParticipationRepository;
    }


    public async Task<Result> Handle( CreateTagsCommand command, CancellationToken token ) {        

        if(command.CreateParticipationDto.QuestionId == Guid.Empty 
            || command.CreateParticipationDto.UserId == Guid.Empty 
            || command.CreateParticipationDto.TagId <= 0)
            return Result.Error("Некорректный аргумент запроса");

        UserTagParticipation tagParticipation = new UserTagParticipation {
            UserId = command.CreateParticipationDto.UserId,
            TagId = command.CreateParticipationDto.TagId,
            LastActiveAt = DateTime.UtcNow,
            QuestionsCreated = command.CreateParticipationDto.IsAnswer ? 0 : 1,
            AnswersWritten = command.CreateParticipationDto.IsAnswer ? 1 : 0
            
        };

        tagParticipation.UserTagParticipationQuestions.Add(new UserTagParticipationQuestion {
            QuestionId = command.CreateParticipationDto.QuestionId
        });

        Result result = await _repository.CreateAsync(tagParticipation, token);

        if(result.IsConflict()) {

            Result<UserTagParticipation?> tag = await _repository.GetUserTagParticipation(command.CreateParticipationDto.UserId, command.CreateParticipationDto.TagId, token);
            if(!tag.IsSuccess)
                return Result.Error(new ErrorList(tag.Errors));

            if(command.CreateParticipationDto.IsAnswer)
                tag.Value!.AnswersWritten += 1;
            else
                tag.Value!.QuestionsCreated += 1;

            tag.Value.UserTagParticipationQuestions.Add(new UserTagParticipationQuestion {
                QuestionId = command.CreateParticipationDto.QuestionId
            });

            Result resultUpdate = await _repository.UpdateAsync(tag, token);

            return resultUpdate.IsSuccess ? Result.Success() : Result.Error(new ErrorList(resultUpdate.Errors));
        }

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
