using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;

namespace TagService.Infrastructure.Repositories;

public class WatchedTagRepository : IWatchedTagRepository {

    private readonly TagServiceDbContext _dbContext;
    private readonly ILogger<WatchedTagRepository> _logger;

    public WatchedTagRepository( TagServiceDbContext dbContext, ILogger<WatchedTagRepository> logger ) {
        _dbContext = dbContext;
        _logger = logger;
    }


    /// <summary> Получить отслеживаемый тэг по tagID и userID. </summary>
    private async Task<Result<WatchedTag>> GetWatchedTagById( int tagId, Guid userId, CancellationToken token ) {

        _logger.LogInformation("GetWatchedTagById started. TagId: {TagId}, UserId: {UserId}", tagId, userId);

        WatchedTag? watchedTag = await _dbContext.WatchedTags
            .Where(wt => wt.TagId == tagId && wt.UserId == userId)
            .FirstOrDefaultAsync(token);

        if(watchedTag is null) {
            _logger.LogWarning(
                "GetWatchedTagById: тэг {TagId} не найден для пользователя {UserId}", tagId, userId);
            return Result<WatchedTag>.Error("Тэг не найден");
        }

        _logger.LogInformation("GetWatchedTagById: тэг {TagId} найден для пользователя {UserId}", tagId, userId);
        return Result<WatchedTag>.Success(watchedTag);
    }


    /// <summary> Получить список отслеживаемых тэгов по userID. </summary>
    public async Task<Result<IEnumerable<WatchedTag>>> GetUserWatchedTagsAsync( Guid userId, CancellationToken token ) {

        _logger.LogInformation("GetUserWatchedTagsAsync started. UserId: {UserId}", userId);

        try {
            List<WatchedTag> tags = await _dbContext.WatchedTags
                .Where(t => t.UserId == userId)
                .Include(t => t.Tag)
                .OrderBy(t => t.Tag.Name)
                .ToListAsync(token);

            _logger.LogInformation(
                "GetUserWatchedTagsAsync: получено {Count} тэгов для пользователя {UserId}", tags.Count, userId);
            return Result<IEnumerable<WatchedTag>>.Success(tags);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetUserWatchedTagsAsync: ошибка базы данных для пользователя {UserId}", userId);
            return Result<IEnumerable<WatchedTag>>.Error("Ошибка базы данных при получении истории изменений вопросов");
        }
    }


    /// <summary> Создать отслеживаемый тэг. </summary>
    public async Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token ) {

        _logger.LogInformation("CreateAsync started. TagId: {TagId}, UserId: {UserId}", watchedTag?.TagId, watchedTag?.UserId);

        if(watchedTag is null) {
            _logger.LogWarning("CreateAsync: аргумент watchedTag равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        bool exist = await _dbContext.WatchedTags.AsNoTracking().AnyAsync(wt => wt.TagId == watchedTag.TagId, token);
        if(exist) {
            _logger.LogWarning("CreateAsync: тэг {TagId} уже отслеживается", watchedTag.TagId);
            return Result.Error("Тэг с таким именем уже отслеживается");
        }

        try {
            await _dbContext.WatchedTags.AddAsync(watchedTag, token);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("CreateAsync: тэг {TagId} успешно добавлен в отслеживаемые", watchedTag.TagId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateAsync: ошибка конкуренции при добавлении тэга {TagId}", watchedTag.TagId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateAsync: ошибка БД при добавлении тэга {TagId}", watchedTag.TagId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить отслеживаемый тэг. </summary>
    public async Task<Result> DeleteAsync( int tagId, Guid userId, CancellationToken token ) {

        _logger.LogInformation("DeleteAsync started. TagId: {TagId}, UserId: {UserId}", tagId, userId);

        Result<WatchedTag> result = await GetWatchedTagById(tagId, userId, token);
        if(!result.IsSuccess) {
            _logger.LogWarning("DeleteAsync: тэг {TagId} не найден для пользователя {UserId}", tagId, userId);
            return Result.Error("Тэг не найден");
        }

        try {
            _dbContext.WatchedTags.Remove(result.Value);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteAsync: тэг {TagId} удалён для пользователя {UserId}", tagId, userId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "DeleteAsync: ошибка конкуренции при удалении тэга {TagId}", tagId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteAsync: ошибка БД при удалении тэга {TagId}", tagId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить все отслеживаемые тэги пользователя. </summary>
    public async Task<Result> DeleteUserTagsAsync( IEnumerable<WatchedTag> watchedTags, CancellationToken token ) {

        _logger.LogInformation("DeleteUserTagsAsync started. Количество тэгов: {Count}", watchedTags?.Count() ?? 0);

        try {
            _dbContext.WatchedTags.RemoveRange(watchedTags);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteUserTagsAsync: удалено {Count} отслеживаемых тэгов", watchedTags.Count());
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "DeleteUserTagsAsync: ошибка конкуренции при удалении тэгов");
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteUserTagsAsync: ошибка БД при удалении тэгов");
            return Result.Error("Ошибка БД");
        }
    }

    


    public async Task<bool> ExistsAsync( Guid userId, int tagId, CancellationToken token ) =>
       await _dbContext.WatchedTags
           .AsNoTracking()
           .AnyAsync(wt => wt.UserId == userId && wt.TagId == tagId, token);
  


    public async Task AddAsync( WatchedTag entity, CancellationToken token ) =>
        await _dbContext.WatchedTags.AddAsync(entity, token);
  

    public async Task SaveChangesAsync( CancellationToken token ) =>
        await _dbContext.SaveChangesAsync(token);


    public async Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken token ) =>
        await _dbContext.Database.BeginTransactionAsync(token);


    public async Task<List<WatchedTag>> GetUserWatchedTagsListAsync( Guid userId, CancellationToken ct ) =>
    await _dbContext.WatchedTags
        .Where(t => t.UserId == userId)
        .ToListAsync(ct);


    public void RemoveRange( IEnumerable<WatchedTag> items ) =>
    _dbContext.WatchedTags.RemoveRange(items);

}
