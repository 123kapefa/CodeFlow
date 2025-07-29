using Abstractions.Commands;
using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.CreateWatchedTag;

public class CreateWatchedTagHandler : ICommandHandler<CreateWatchedTagCommand> {

    private readonly IWatchedTagRepository _watchedTagRepository;
    private readonly ITagRepository _tagRepository;

    public CreateWatchedTagHandler( IWatchedTagRepository watchedTagRepository, ITagRepository tagRepository ) {
        _watchedTagRepository = watchedTagRepository;
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle( CreateWatchedTagCommand command, CancellationToken token ) {

        if(command.UserId == Guid.Empty)
            return Result.Error("ID пользователя не может быть пустым");

        if(command.TagId <= 0)
            return Result.Error("ID тэга некорректный");

        
        if(await _watchedTagRepository.ExistsAsync(command.UserId, command.TagId, token))
            return Result.Conflict("Тэг уже в отслеживаемых.");


        Result<Tag> tagRes = await _tagRepository.GetTagByIdAsync(command.TagId, token);
        if(!tagRes.IsSuccess)
            return Result.Error(new ErrorList(tagRes.Errors));

        Tag tag = tagRes.Value;

        WatchedTag watched = WatchedTag.Create(command.UserId, command.TagId);

        await using var tx = await _watchedTagRepository.BeginTransactionAsync(token);
        try {           
            await _watchedTagRepository.AddAsync(watched, token);
            
            tag.CountWotchers += 1;
           
            await _watchedTagRepository.SaveChangesAsync(token);

            await tx.CommitAsync(token);
            return Result.Success();
        }        
        catch {
            await tx.RollbackAsync(token);
            return Result.Error("Ошибка при добавлении тэга в отслеживаемые.");
        }
    }

}
