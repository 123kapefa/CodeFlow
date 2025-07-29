using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Linq;
using TagService.Domain.Entities;
using TagService.Domain.Filters;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;
using TagService.Infrastructure.Extensions;

namespace TagService.Infrastructure.Repositories;

public class UserTagParticipationRepository : IUserTagParticipationRepository {

    private readonly TagServiceDbContext _dbContext;
    private readonly ILogger<UserTagParticipationRepository> _logger;

    public UserTagParticipationRepository( TagServiceDbContext dbContext, ILogger<UserTagParticipationRepository> logger ) {
        _dbContext = dbContext;
        _logger = logger;
    }


    /// <summary> Получить список тэгов пользователя (постранично / сортировка). </summary>
    public async Task<Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>> GetTagsAsync(
        Guid userId, PageParams pageParams, SortParams sortParams, CancellationToken token ) {

        _logger.LogInformation(
            "GetTagsAsync started. UserId: {UserId}, PageParams: {@PageParams}, SortParams: {@SortParams}",
            userId, pageParams, sortParams);

        try {
            var tags = await _dbContext.UserTagParticipations
                .Where(t => t.UserId == userId)
                .Include(t => t.Tag)
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            _logger.LogInformation(
                "GetTagsAsync: получено {Count} тэгов участия для пользователя {UserId}",
                tags.Value.items.Count(), userId);
            return Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>.Success(tags);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetTagsAsync: ошибка базы данных");
            return Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Получить список тэгов пользователя. </summary>
    private async Task<IEnumerable<UserTagParticipation>> GetUserTagsAsync( Guid userId, CancellationToken token ) {

        _logger.LogInformation("GetUserTagsAsync started (private). UserId: {UserId}", userId);

        List<UserTagParticipation> tags =
            await _dbContext.UserTagParticipations.Where(t => t.UserId == userId).ToListAsync(token);

        _logger.LogInformation("GetUserTagsAsync: найдено {Count} тэгов участия для пользователя {UserId}",
            tags.Count, userId);
        return tags;
    }

    public async Task<IEnumerable<UserTagParticipation>> GetUserTagsAsync( 
        Guid userId, IEnumerable<int> tagIds, CancellationToken token ) {

        var parts = await _dbContext.UserTagParticipations
            .Where(p => p.UserId == userId && tagIds.Contains(p.TagId))
            .ToListAsync(token);
        
        return parts;
    }


    /// <summary> Получить тэг пользователя по tagId. </summary>
    public async Task<Result<UserTagParticipation?>> GetUserTagParticipation( Guid userId, int tagId, CancellationToken token ) {

        _logger.LogInformation("GetUserTagParticipation started. UserId: {UserId}, TagId: {TagId}", userId, tagId);

        UserTagParticipation? tag = await _dbContext.UserTagParticipations
            .Where(t => t.UserId == userId && t.TagId == tagId)
            .Include(t => t.UserTagParticipationQuestions)
            .FirstOrDefaultAsync();

        if(tag == null) {
            _logger.LogWarning(
                "GetUserTagParticipation: запись не найдена (UserId: {UserId}, TagId: {TagId})", userId, tagId);
            return Result<UserTagParticipation?>.Error("Записи не найдены");
        }

        _logger.LogInformation("GetUserTagParticipation: запись найдена (UserId: {UserId}, TagId: {TagId})", userId, tagId);
        return Result<UserTagParticipation?>.Success(tag);
    }


    /// <summary> Создать запись участия в тэге. </summary>
    public async Task<Result> CreateAsync( UserTagParticipation tagParticipation, CancellationToken token ) {

        _logger.LogInformation("CreateAsync started. UserId: {UserId}, TagId: {TagId}",
            tagParticipation?.UserId, tagParticipation?.TagId);

        if(tagParticipation is null) {
            _logger.LogWarning("CreateAsync: аргумент tagParticipation равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        bool exist = await _dbContext.UserTagParticipations
            .AsNoTracking()
            .AnyAsync(t => t.TagId == tagParticipation.TagId && t.UserId == tagParticipation.UserId, token);

        if(exist) {
            _logger.LogWarning(
                "CreateAsync: запись уже существует (UserId: {UserId}, TagId: {TagId})",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Conflict($"Запись для тэга {tagParticipation.TagId} для данного пользователя уже существует.");
        }

        try {
            await _dbContext.UserTagParticipations.AddAsync(tagParticipation, token);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "CreateAsync: запись (UserId: {UserId}, TagId: {TagId}) успешно создана",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateAsync: ошибка конкуренции (UserId: {UserId}, TagId: {TagId})",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateAsync: ошибка БД (UserId: {UserId}, TagId: {TagId})",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновить запись участия в тэге. </summary>
    public async Task<Result> UpdateAsync( UserTagParticipation tagParticipation, CancellationToken token ) {

        _logger.LogInformation("UpdateAsync started. UserId: {UserId}, TagId: {TagId}",
            tagParticipation?.UserId, tagParticipation?.TagId);

        if(tagParticipation is null) {
            _logger.LogWarning("UpdateAsync: аргумент tagParticipation равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            _dbContext.UserTagParticipations.Update(tagParticipation);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "UpdateAsync: запись (UserId: {UserId}, TagId: {TagId}) успешно обновлена",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, 
                "UpdateAsync: запись была изменена или удалена другим пользователем (UserId: {UserId}, TagId: {TagId})",  
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex,
                "UpdateAsync: ошибка БД при обновлении (UserId: {UserId}, TagId: {TagId})",
                tagParticipation.UserId, tagParticipation.TagId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить все тэги пользователя по userId. </summary>
    public async Task<Result> DeleteUserTagsAsync( Guid userId, CancellationToken token ) {

        _logger.LogInformation("DeleteUserTagsAsync started. UserId: {UserId}", userId);

        IEnumerable<UserTagParticipation> tags = await GetUserTagsAsync(userId, token);

        try {
            _dbContext.UserTagParticipations.RemoveRange(tags);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "DeleteUserTagsAsync: удалено {Count} записей для пользователя {UserId}",
                tags.Count(), userId);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex,
                "DeleteUserTagsAsync: запись была изменена или удалена другим пользователем (UserId: {UserId})",
                userId);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteUserTagsAsync: ошибка БД при удалении тэгов пользователя {UserId}", userId);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить записи из UserTagParticipationQuestions для определенных тэгов. </summary>
    public async Task<Result> DeleteUserParticipationTags( Guid questionId, IEnumerable<Guid> participationIds, CancellationToken token ) {

        var forDel = await _dbContext.UserTagParticipationQuestions
            .Where(q => q.QuestionId == questionId)
            .Take(participationIds.Count()).ToListAsync(token);

        _dbContext.RemoveRange(forDel);

        return Result.Success();        
    }


    public void DeleteTagParticipation(UserTagParticipation userTag) => 
        _dbContext.UserTagParticipations.Remove(userTag);


    /// <summary> Получить словарь UserTagParticipations по UserId и IEnumerable<int> TagIds. </summary>
    public async Task<Dictionary<int, UserTagParticipation>> GetByUserAndTagIdsAsync(
        Guid userId, IEnumerable<int> tagIds, CancellationToken ct ) {
        var ids = tagIds.Distinct().ToList();
        var items = await _dbContext.UserTagParticipations
            .Where(p => p.UserId == userId && ids.Contains(p.TagId))
            .ToListAsync(ct);

        return items.ToDictionary(p => p.TagId);
    }


    public async Task AddRangeAsync( IEnumerable<UserTagParticipation> items, CancellationToken ct )
        => await _dbContext.UserTagParticipations.AddRangeAsync(items, ct);


    public async Task AddQuestionsRangeAsync( IEnumerable<UserTagParticipationQuestion> items, CancellationToken ct )
       => await _dbContext.UserTagParticipationQuestions.AddRangeAsync(items, ct);


    public async Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken token ) =>
        await _dbContext.Database.BeginTransactionAsync(token);


    public async Task SaveChangesAsync( CancellationToken token )
        => await _dbContext.SaveChangesAsync(token);


    public async Task<List<UserTagParticipation>> GetByUserAsync( Guid userId, CancellationToken ct ) =>
    await _dbContext.UserTagParticipations
        .Where(t => t.UserId == userId)
        .ToListAsync(ct);


    public void RemoveRange( IEnumerable<UserTagParticipation> items ) =>
        _dbContext.UserTagParticipations.RemoveRange(items);

}
