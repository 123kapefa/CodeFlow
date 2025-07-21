using Abstractions.Commands;

using Ardalis.Result;
using Contracts.TagService;

using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.GetTagById;

public class GetTagByIdHandler : ICommandHandler<TagDTO, GetTagByIdCommand> {

    private ITagRepository _tagRepository;

    public GetTagByIdHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }


    public async Task<Result<TagDTO>> Handle( GetTagByIdCommand command, CancellationToken token ) {

        if(command.TagId <= 0)
            return Result<TagDTO>.Error("Некорректный аргумент запроса");

        Result<Tag> result = await _tagRepository.GetTagByIdAsync(command.TagId, token);

        if(!result.IsSuccess)
            return Result<TagDTO>.Error(new ErrorList(result.Errors));

        //TagDTO resultDto = new TagDTO {
        //    Id = result.Value.Id,
        //    Name = result.Value.Name,
        //    Description = result.Value.Description,
        //    CreatedAt = DateTime.Now,
        //    CountQuestion = result.Value.CountQuestion,
        //    CountWotchers = result.Value.CountWotchers,
        //    DailyRequestCount = result.Value.DailyRequestCount,
        //    WeeklyRequestCount = result.Value.WeeklyRequestCount
        //};

        TagDTO resultDto = TagDTO.Create(
            result.Value.Id,
            result.Value.Name,
            result.Value.Description,
            DateTime.Now,
            result.Value.CountQuestion,
            result.Value.CountWotchers,
            result.Value.DailyRequestCount,
            result.Value.WeeklyRequestCount
        );

        return Result.Success(resultDto);
    }

}
