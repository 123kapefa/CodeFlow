using Ardalis.Result;
using Contracts.Commands;
using TagService.Application.DTO;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.GetTags;

public class GetTagsHandler : ICommandHandler<PagedResult<IEnumerable<TagDTO>>, GetTagsCommand> {

    private readonly ITagRepository _tagRepository;

    public GetTagsHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }


    public async Task<Result<PagedResult<IEnumerable<TagDTO>>>> Handle( GetTagsCommand command, CancellationToken token ) {

        var tagsResult = await _tagRepository.GetTagsAsync( command.PageParams, command.SortParams, token);
        if(!tagsResult.IsSuccess)
            return Result<PagedResult<IEnumerable<TagDTO>>>.Error(new ErrorList(tagsResult.Errors));

        IEnumerable<TagDTO> tags = tagsResult.Value.items.Select(i => new TagDTO {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            CreatedAt = i.CreatedAt,
            CountQuestion = i.CountQuestion,
            CountWotchers = i.CountWotchers,
            DailyRequestCount = i.DailyRequestCount,
            WeeklyRequestCount = i.WeeklyRequestCount
        });

        return Result<PagedResult<IEnumerable<TagDTO>>>
            .Success(new PagedResult<IEnumerable<TagDTO>>(tagsResult.Value.pageInfo, tags));
    }

}
