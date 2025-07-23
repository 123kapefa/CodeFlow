using Abstractions.Commands;

using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.UpdateTagWatchers;

public class UpdateTagWatchersHandler : ICommandHandler<UpdateTagWatchersCommand> {

    private readonly ITagRepository _tagRepository;

    public UpdateTagWatchersHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }


    public async Task<Result> Handle( UpdateTagWatchersCommand command, CancellationToken token ) {

        if(command.TagId <= 0)
            return Result.Error("Некорректный аргумент запроса");

        if(command.Count == 0)
            return Result.Error("Количество не может быть равно 0");

        Result<Tag> resultTag = await _tagRepository.GetTagByIdAsync(command.TagId, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        resultTag.Value.CountWotchers += command.Count;

        Result result = await _tagRepository.UpdateTagAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
