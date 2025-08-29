using Ardalis.Result;

using Abstractions.Commands;

using Contracts.DTOs.TagService;

using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.GetTagByName;

public class GetTagByNameHandler : ICommandHandler<TagDTO, GetTagByNameCommand> {

    private readonly ITagRepository _tagRepository;

    public GetTagByNameHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }


    public async Task<Result<TagDTO>> Handle( GetTagByNameCommand command, CancellationToken token ) {

        if(string.IsNullOrEmpty(command.Name))
            return Result.Error("Не корректный аргумент запроса");

        var result = await _tagRepository.GetTagByNameAsync(command.Name, token);
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
