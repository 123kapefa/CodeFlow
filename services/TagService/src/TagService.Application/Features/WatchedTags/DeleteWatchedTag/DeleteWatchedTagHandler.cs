using Ardalis.Result;
using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.DeleteWatchedTag;

public class DeleteWatchedTagHandler : ICommandHandler<DeleteWatchedTagCommand> {

    private readonly IWatchedTagRepository _repository;

    public DeleteWatchedTagHandler( IWatchedTagRepository repository ) {
        _repository = repository;
    }

    public async Task<Result> Handle( DeleteWatchedTagCommand command, CancellationToken token ) {

        if(command.TagId <= 0)
            return Result.Error("ID тэга некорректный");

        if(command.UserId == Guid.Empty)
            return Result.Error("ID пользователя не может быть пустым");

        Result result = await _repository.DeleteAsync(command.TagId, command.UserId, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
