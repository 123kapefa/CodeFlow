using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TagService.Domain.Entities;
using TagService.Domain.Filters;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;
using TagService.Infrastructure.Extensions;

namespace TagService.Infrastructure.Repositories;

public class TagRepository : ITagRepository {

    private readonly TagServiceDbContext _dbContext;
    private readonly ILogger<TagRepository> _logger;

    public TagRepository( TagServiceDbContext dbContext, ILogger<TagRepository> logger ) {
        _dbContext = dbContext;
        _logger = logger;
    }


    /// <summary> Получить тэг по ID. </summary>
    public async Task<Result<Tag>> GetTagByIdAsync( int tagId, CancellationToken token ) {

        _logger.LogInformation("GetTagByIdAsync started. TagId: {TagId}", tagId);

        Tag? tag = await _dbContext.Tags.FindAsync(tagId, token);

        if(tag is null) {
            _logger.LogWarning("GetTagByIdAsync: тэг {TagId} не найден", tagId);
            return Result<Tag>.Error("Тэг не найден");
        }

        _logger.LogInformation("GetTagByIdAsync: тэг {TagId} успешно найден", tagId);
        return Result<Tag>.Success(tag);
    }


    /// <summary> Получить тэг по Name. </summary>
    public async Task<Result<Tag>> GetTagByNameAsync( string name, CancellationToken token ) {

        _logger.LogInformation("GetTagByNameAsync started. TagName: {TagName}", name);

        Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, token);

        if(tag is null) {
            _logger.LogWarning("GetTagByNameAsync: тэг с именем {TagName} не найден", name);
            return Result<Tag>.Error("Тэг не найден");
        }

        _logger.LogInformation("GetTagByNameAsync: тэг {TagName} успешно найден", name);
        return Result<Tag>.Success(tag);
    }
    
    
    public async Task<Result<List<Tag>>> GetTagsByIdAsync(List<int> ids, CancellationToken token) {

        List<Tag> tags = await _dbContext.Tags
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(token);

        return Result<List<Tag>>.Success(tags);
    }


    /// <summary> Получить список тэгов (постранично / сортировка). </summary>
    public async Task<Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>> GetTagsAsync(
        PageParams pageParams,
        SortParams sortParams,
        CancellationToken token ) {

        _logger.LogInformation(
            "GetTagsAsync started. PageParams: {@PageParams}, SortParams: {@SortParams}", pageParams, sortParams);

        try {
            var tags = await _dbContext.Tags
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            _logger.LogInformation("GetTagsAsync: получено {Count} тэгов", tags.Value.items.Count());
            return Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>.Success(tags);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "GetTagsAsync: ошибка базы данных");
            return Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Создать тэг. </summary>
    public async Task<Result> CreateTagAsync( Tag tag, CancellationToken token ) {

        _logger.LogInformation("CreateTagAsync started. TagName: {TagName}", tag?.Name);

        if(tag is null) {
            _logger.LogWarning("CreateTagAsync: аргумент tag равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        bool exist = await _dbContext.Tags.AsNoTracking().AnyAsync(t => t.Name == tag.Name, token);
        if(exist) {
            _logger.LogWarning("CreateTagAsync: тэг с именем {TagName} уже существует", tag.Name);
            return Result.Error("Тэг с таким именем уже существует");
        }

        try {
            await _dbContext.Tags.AddAsync(tag, token);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("CreateTagAsync: тэг {TagName} успешно создан", tag.Name);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "CreateTagAsync: ошибка конкуренции при создании тэга {TagName}", tag.Name);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "CreateTagAsync: ошибка БД при создании тэга {TagName}", tag.Name);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Обновить тэг. </summary>
    public async Task<Result> UpdateTagAsync( Tag tag, CancellationToken token ) {

        _logger.LogInformation("UpdateTagAsync started. TagId: {TagId}", tag?.Id);

        if(tag is null) {
            _logger.LogWarning("UpdateTagAsync: аргумент tag равен null");
            return Result.Error("Аргумент запроса не может быть null");
        }

        try {
            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("UpdateTagAsync: тэг {TagId} успешно обновлён", tag.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "UpdateTagAsync: тэг {TagId} был изменён или удалён другим пользователем", tag.Id);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "UpdateTagAsync: ошибка БД при обновлении тэга {TagId}", tag.Id);
            return Result.Error("Ошибка БД");
        }
    }


    /// <summary> Удалить тэг (каскадно). </summary>
    public async Task<Result> DeleteTagAsync( Tag tag, CancellationToken token ) {

        _logger.LogInformation("DeleteTagAsync started. TagId: {TagId}", tag?.Id);

        try {
            _dbContext.Tags.Remove(tag);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("DeleteTagAsync: тэг {TagId} успешно удалён", tag.Id);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "DeleteTagAsync: тэг {TagId} был изменён или удалён другим пользователем", tag.Id);
            return Result.Error("Тэг был изменён или удалён другим пользователем");
        }
        catch(DbUpdateException ex) {
            _logger.LogError(ex, "DeleteTagAsync: ошибка БД при удалении тэга {TagId}", tag.Id);
            return Result.Error("Ошибка БД");
        }
    }

    public async Task AddRangeAsync( IEnumerable<Tag> tags, CancellationToken token )
        => await _dbContext.Tags.AddRangeAsync(tags, token);

    public async Task SaveChangesAsync( CancellationToken token )
        => await _dbContext.SaveChangesAsync(token);

    public async Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken token ) =>
        await _dbContext.Database.BeginTransactionAsync(token);

}
