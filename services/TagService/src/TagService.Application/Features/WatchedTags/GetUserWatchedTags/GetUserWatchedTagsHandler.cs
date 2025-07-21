using Abstractions.Commands;

using Ardalis.Result;
using Contracts.TagService;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.GetUserWatchedTags;

public class GetUserWatchedTagsHandler : ICommandHandler<IEnumerable<WatchedTagDTO>, GetUserWatchedTagsCommand> {

    private readonly IWatchedTagRepository _repository;

    public GetUserWatchedTagsHandler( IWatchedTagRepository repository ) {
        _repository = repository;
    }


    public async Task<Result<IEnumerable<WatchedTagDTO>>> Handle( 
        GetUserWatchedTagsCommand command, 
        CancellationToken token ) {

        if(command.UserId == Guid.Empty)
            return Result<IEnumerable<WatchedTagDTO>>.Error("ID пользователя не может быть пустым");

        Result<IEnumerable<WatchedTag>> result = await _repository.GetUserWatchedTagsAsync(command.UserId, token);
        if(!result.IsSuccess)
            return Result.Error(new ErrorList(result.Errors));

        //IEnumerable<WatchedTagDTO> watchedTags = result.Value.Select(x => new WatchedTagDTO {
        //    Id = x.Id,
        //    UserId = x.UserId,
        //    TagId = x.TagId,
        //    TagName = x.Tag.Name
        //}).ToList();

        IEnumerable<WatchedTagDTO> watchedTags = result.Value.Select(x => WatchedTagDTO.Create(
            x.Id, x.UserId, x.TagId, x.Tag.Name )).ToList();

        return result.IsSuccess ? Result.Success(watchedTags) : Result.Error(new ErrorList(result.Errors));
    }

}
