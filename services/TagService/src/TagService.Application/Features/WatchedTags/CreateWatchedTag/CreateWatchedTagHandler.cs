using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.CreateWatchedTag;

public class CreateWatchedTagHandler : ICommandHandler<CreateWatchedTagCommand> {

    private readonly IWatchedTagRepository _watchedTagRepository;

    public CreateWatchedTagHandler( IWatchedTagRepository watchedTagRepository ) {
        _watchedTagRepository = watchedTagRepository;
    }


    public async Task<Result> Handle( CreateWatchedTagCommand command, CancellationToken token ) {

        if(command.UserId == Guid.Empty)
            return Result.Error("ID пользователя не может быть пустым");

        if(command.TagId <= 0)
            return Result.Error("ID тэга некорректный");

        //WatchedTag watchedTag = new WatchedTag {
        //    UserId = command.UserId,
        //    TagId = command.TagId,
        //};

        WatchedTag watchedTag = WatchedTag.Create(command.UserId, command.TagId);

        Result result = await _watchedTagRepository.CreateAsync(watchedTag, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
