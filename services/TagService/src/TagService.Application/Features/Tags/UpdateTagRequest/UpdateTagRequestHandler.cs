using Ardalis.Result;
using Contracts.Commands;
using System.Globalization;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.UpdateTagRequest;

public class UpdateTagRequestHandler : ICommandHandler<UpdateTagRequestCommand> {

    private readonly ITagRepository _tagRepository;

    public UpdateTagRequestHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }

    //TODO ВНИМАТЕЛЬНЕЙ ПРОВЕРИТЬ !!!! ))
    public async Task<Result> Handle( UpdateTagRequestCommand command, CancellationToken token ) {

        if(string.IsNullOrEmpty(command.Name))
            return Result.Error("Некорректный аргумент запроса");

        Result<Tag> resultTag = await _tagRepository.GetTagByNameAsync(command.Name, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        DateTime now = DateTime.UtcNow;

        // проверка дня
        if(resultTag.Value.DailyCountUpdatedAt is null || (now.Date > resultTag.Value.DailyCountUpdatedAt.Value.Date)) {
            resultTag.Value.DailyCountUpdatedAt = now;
            resultTag.Value.DailyRequestCount = 0;
        }
            

        int currWeek = ISOWeek.GetWeekOfYear(now);
        var lastUpdate = resultTag.Value.WeeklyCountUpdatedAt;

        // проверка недели
        if(lastUpdate is null || ISOWeek.GetWeekOfYear(lastUpdate.Value) < currWeek || lastUpdate.Value.Year < now.Year) {
            resultTag.Value.WeeklyCountUpdatedAt = now;
            resultTag.Value.WeeklyRequestCount = 0;
        }
            

        resultTag.Value.DailyRequestCount += 1;
        resultTag.Value.WeeklyRequestCount += 1;
        resultTag.Value.CountQuestion += 1;

        Result result = await _tagRepository.UpdateTagAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
