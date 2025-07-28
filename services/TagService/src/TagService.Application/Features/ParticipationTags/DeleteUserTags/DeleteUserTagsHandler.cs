using Abstractions.Commands;

using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.DeleteUserTags;

public class DeleteUserTagsHandler : ICommandHandler<DeleteUserTagsCommand> {

    private readonly IUserTagParticipationRepository _participationRepo;
    private readonly IWatchedTagRepository _watchedRepo;
    private readonly ITagRepository _tagRepo;

    public DeleteUserTagsHandler(
        IUserTagParticipationRepository participationRepo,
        IWatchedTagRepository watchedRepo,
        ITagRepository tagRepo ) {

        _participationRepo = participationRepo;
        _watchedRepo = watchedRepo;
        _tagRepo = tagRepo;
    }


    public async Task<Result> Handle( DeleteUserTagsCommand command, CancellationToken token ) {
        if(command.UserId == Guid.Empty)
            return Result.Error("Некорректный аргумент запроса");

        // Получаем все сущности пользователя
        List<UserTagParticipation> participations = await _participationRepo.GetByUserAsync(command.UserId, token);
        List<WatchedTag> watched = await _watchedRepo.GetUserWatchedTagsListAsync(command.UserId, token);

        // Готовим декременты по TagId для CountWotchers
        Dictionary<int, int> decMap = watched
            .GroupBy(w => w.TagId)
            .ToDictionary(g => g.Key, g => g.Count());

        // Подтягиваем теги для декремента
        List<Tag> tagsToUpdate = [];
        if(decMap.Count > 0) {
            List<int?> ids = decMap.Keys.Select(id => (int?)id).ToList();
            Result<List<Tag>> tagsRes = await _tagRepo.GetTagsByIdAsync(ids, token);
            if(!tagsRes.IsSuccess)
                return Result.Error(new ErrorList(tagsRes.Errors));

            tagsToUpdate = tagsRes.Value;
        }

        await using var tx = await _watchedRepo.BeginTransactionAsync(token);
        try {

            if(participations.Count > 0)
                _participationRepo.RemoveRange(participations);


            foreach(Tag tag in tagsToUpdate) {
                int dec = decMap[tag.Id];
                int newVal = tag.CountWotchers - dec;
                tag.CountWotchers = newVal > 0 ? newVal : 0;
            }

            if(watched.Count > 0)
                _watchedRepo.RemoveRange(watched);

            await _watchedRepo.SaveChangesAsync(token);

            await tx.CommitAsync(token);
            return Result.Success();
        }
        catch {
            await tx.RollbackAsync(token);
            return Result.Error("Ошибка при удалении данных пользователя по тэгам.");
        }
    }

}
