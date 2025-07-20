using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using TagService.Domain.Entities;
using TagService.Domain.Filters;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;
using TagService.Infrastructure.Extensions;

namespace TagService.Infrastructure.Repositories;

public class UserTagParticipationRepository : IUserTagParticipationRepository {

    private readonly TagServiceDbContext _dbContext;

    public UserTagParticipationRepository( TagServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получить список тэгов пользователя(постранично / сортировка) </summary>
    public async Task<Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>> GetTagsAsync(
        Guid userId, PageParams pageParams, SortParams sortParams, CancellationToken token ) {

        try {
            var tags = await _dbContext.UserTagParticipations
                .Where(t => t.UserId == userId)
                .Include(t => t.Tag)
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            return Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>.Success(tags);
        }
        catch(Exception) {
            return Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Получить список тэгов пользователя. </summary>
    private async Task<IEnumerable<UserTagParticipation>> GetUserTagsAsync( Guid userId, CancellationToken token ) {

        List<UserTagParticipation> tags = 
            await _dbContext.UserTagParticipations.Where(t => t.UserId == userId).ToListAsync(token);

        return tags;
    }


    /// <summary> Получить тэгов пользователя по tagId. </summary>
    public async Task<Result<UserTagParticipation?>> GetUserTagParticipation( Guid userId, int tagId, CancellationToken token ) {

        UserTagParticipation? tag = await _dbContext.UserTagParticipations
            .Where(t => t.UserId == userId && t.TagId == tagId)
            .Include(t => t.UserTagParticipationQuestions)
            .FirstOrDefaultAsync();

        return tag != null ? Result<UserTagParticipation?>.Success(tag) : Result<UserTagParticipation?>.Error("Записи не найдены");
    }


    /// <summary> Создать тэг. </summary>
    public async Task<Result> CreateAsync( UserTagParticipation tagParticipation, CancellationToken token ) {

        if(tagParticipation is null)
            return Result.Error("Аргумент запроса не может быть null");

        bool exist = await _dbContext.UserTagParticipations
            .AsNoTracking()
            .AnyAsync(t => t.TagId == tagParticipation.TagId && t.UserId == tagParticipation.UserId, token);
        if(exist)
            return Result.Conflict($"Запись для тэга {tagParticipation.TagId} для данного пользователя уже существует.");

        try {
            await _dbContext.UserTagParticipations.AddAsync(tagParticipation, token);
            await _dbContext.SaveChangesAsync(token);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновить тэг. </summary>
    public async Task<Result> UpdateAsync( UserTagParticipation tagParticipation, CancellationToken token ) {

        if(tagParticipation is null)
            return Result.Error("Аргумент запроса не может быть null");

        try {
            _dbContext.UserTagParticipations.Update(tagParticipation);
            await _dbContext.SaveChangesAsync(token);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить все тэги пользователя по userId. </summary>
    public async Task<Result> DeleteUserTagsAsync( Guid userId, CancellationToken token ) {
        IEnumerable<UserTagParticipation> tags = await GetUserTagsAsync(userId, token);

        try {
            _dbContext.UserTagParticipations.RemoveRange(tags);
            await _dbContext.SaveChangesAsync(token);

            return Result.Success();
        }
        catch(DbUpdateConcurrencyException) {
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException) {
            return Result.Error("Ошибка БД");
        }
    }   
   
}
