using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.DeleteUserWatchedTags;

public class DeleteUserWatchedTagsHandler : ICommandHandler<DeleteUserWatchedTagsCommand> {

    private readonly IWatchedTagRepository _repository;

    public DeleteUserWatchedTagsHandler( IWatchedTagRepository repository ) {
        _repository = repository;
    }


    public async Task<Result> Handle( DeleteUserWatchedTagsCommand command, CancellationToken token ) {

        if(command.UserId == Guid.Empty)
            return Result.Error("ID пользователя не может быть пустым");

        Result<IEnumerable<WatchedTag>> userTagsResult = await _repository.GetUserWatchedTagsAsync(command.UserId, token);
        if(!userTagsResult.IsSuccess)
            return Result.Error(new ErrorList(userTagsResult.Errors));

        Result result = await _repository.DeleteUserTagsAsync(userTagsResult.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
