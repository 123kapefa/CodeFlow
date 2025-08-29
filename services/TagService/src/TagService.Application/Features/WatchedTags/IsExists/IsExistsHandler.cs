using Abstractions.Commands;
using Ardalis.Result;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.IsExists;

public class IsExistsHandler : ICommandHandler<bool, IsExistsCommand> {

    private readonly IWatchedTagRepository _repository;

    public IsExistsHandler( IWatchedTagRepository repository ) {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle( IsExistsCommand command, CancellationToken cancellationToken ) {
        if(command.UserId == Guid.Empty)
            return Result<bool>.Error("ID пользователя не может быть пустым");
        if(command.TagId <= 0)
            return Result<bool>.Error("ID тэга должен быть больше нуля");

        return await _repository.ExistsAsync(command.UserId, command.TagId, cancellationToken);
    }
}
