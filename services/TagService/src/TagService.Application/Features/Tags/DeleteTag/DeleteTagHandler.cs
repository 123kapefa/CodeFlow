using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.DeleteTag;

public class DeleteTagHandler : ICommandHandler<DeleteTagCommand> {

    private readonly ITagRepository _tagRepository;

    public DeleteTagHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle( DeleteTagCommand command, CancellationToken token ) {

        if(command.TagId <= 0)
            return Result.Error("Не корректный аргумент запроса");

        Result<Tag> resultTag = await _tagRepository.GetTagByIdAsync(command.TagId, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        Result result = await _tagRepository.DeleteTagAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
