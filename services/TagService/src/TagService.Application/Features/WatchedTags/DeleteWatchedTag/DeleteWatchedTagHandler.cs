using Abstractions.Commands;

using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.DeleteWatchedTag;

public class DeleteWatchedTagHandler : ICommandHandler<DeleteWatchedTagCommand> {

    private readonly IWatchedTagRepository _repository;
    private readonly ITagRepository _tagRepository;

    public DeleteWatchedTagHandler( IWatchedTagRepository repository, ITagRepository tagRepository ) {
        _repository = repository;
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle( DeleteWatchedTagCommand command, CancellationToken token ) {

        if(command.TagId <= 0)
            return Result.Error("ID тэга некорректный");

        if(command.UserId == Guid.Empty)
            return Result.Error("ID пользователя не может быть пустым");

        Result result = await _repository.DeleteAsync(command.TagId, command.UserId, token);

        Result<Tag> resultTag = await _tagRepository.GetTagByIdAsync(command.TagId, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        if(resultTag.Value.CountWotchers > 0)
            resultTag.Value.CountWotchers -= 1;

        Result updateRes = await _tagRepository.UpdateTagAsync(resultTag.Value, token);
        if(!updateRes.IsSuccess)
            return Result.Error(new ErrorList(updateRes.Errors));

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
