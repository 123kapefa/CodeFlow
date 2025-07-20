using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;

namespace TagService.Infrastructure.Repositories;

public class WatchedTagRepository : IWatchedTagRepository {

    private readonly TagServiceDbContext _dbContext;

    public WatchedTagRepository( TagServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получить отслеживаемый тэг по tagID и userID. </summary>
    private async Task<Result<WatchedTag>> GetWatchedTagById( int tagId, Guid userId, CancellationToken token ) {
        WatchedTag? watchedTag = await _dbContext.WatchedTags
            .Where(wt => wt.TagId == tagId && wt.UserId == userId)
            .FirstOrDefaultAsync( token );

        return watchedTag is null ? Result<WatchedTag>.Error("Тэг не найден") : Result<WatchedTag>.Success( watchedTag );
    }


    /// <summary> Получить список отслеживаемых тэгов по userID. </summary>
    public async Task<Result<IEnumerable<WatchedTag>>> GetUserWatchedTagsAsync( Guid userId, CancellationToken token ) {

        try {
            List<WatchedTag> tags = await _dbContext.WatchedTags
                .Where(t => t.UserId == userId)                
                .Include(t => t.Tag)
                .OrderBy(t => t.Tag.Name)
                .ToListAsync(token);

            return Result<IEnumerable<WatchedTag>>.Success(tags);
        }
        catch(Exception) {
            return Result<IEnumerable<WatchedTag>>.Error("Ошибка базы данных при получении истории изменений вопросов");
        }        
    }


    /// <summary> Создать отслеживаемый тэг. </summary>
    public async Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token ) {

        if(watchedTag is null)
            return Result.Error("Аргумент запроса не может быть null");

        bool exist = await _dbContext.WatchedTags.AsNoTracking().AnyAsync(wt => wt.TagId == watchedTag.TagId, token);
        if(exist)
            return Result.Error("Тэг с таким именем уже отслеживается");

        try {
            await _dbContext.WatchedTags.AddAsync(watchedTag, token);
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


    /// <summary> Удалить отслеживаемый тэг. </summary>
    public async Task<Result> DeleteAsync( int tagId, Guid userId, CancellationToken token ) {

        Result<WatchedTag> result = await GetWatchedTagById(tagId, userId, token);
        if(!result.IsSuccess)
            return Result.Error("Тэг не найден");

        try {
            _dbContext.WatchedTags.Remove(result.Value);
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


    /// <summary> Удалить все отслеживаемые тэги пользователя. </summary>
    public async Task<Result> DeleteUserTagsAsync( IEnumerable<WatchedTag> watchedTags, CancellationToken token ) {

        try {
            _dbContext.WatchedTags.RemoveRange(watchedTags);
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
