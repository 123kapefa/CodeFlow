using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.DeleteUserTags;

public class DeleteUserTagsHandler : ICommandHandler<DeleteUserTagsCommand> {

    private readonly IUserTagParticipationRepository _repository;

    public DeleteUserTagsHandler( IUserTagParticipationRepository repository ) {
        _repository = repository;
    }


    public async Task<Result> Handle( DeleteUserTagsCommand command, CancellationToken token ) {

        if(command.UserId == Guid.Empty)
            return Result.Error("Некорректный аргумент запроса");

        Result result = await _repository.DeleteUserTagsAsync(command.UserId, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
