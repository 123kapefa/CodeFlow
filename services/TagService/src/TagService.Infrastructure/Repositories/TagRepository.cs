using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using TagService.Domain.Entities;
using TagService.Domain.Filters;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;
using TagService.Infrastructure.Extensions;

namespace TagService.Infrastructure.Repositories;

public class TagRepository : ITagRepository {

    private readonly TagServiceDbContext _dbContext;

    public TagRepository( TagServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получить тэг по ID. </summary>
    public async Task<Result<Tag>> GetTagByIdAsync( int tagId, CancellationToken token ) {

        Tag? tag = await _dbContext.Tags.FindAsync( tagId, token );

        return tag is null ? Result.Error("Тэг не найден") : Result.Success(tag);
    }


    /// <summary> Получить тэг по Name. </summary>
    public async Task<Result<Tag>> GetTagByNameAsync( string name, CancellationToken token ) {

        Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, token);

        return tag is null ? Result.Error("Тэг не найден") : Result.Success(tag);
    }


    /// <summary> Получить список тэгов(постранично / сортировка) </summary>
    public async Task<Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>> GetTagsAsync( 
        PageParams pageParams, 
        SortParams sortParams, 
        CancellationToken token ) {

        try {
            var tags = await _dbContext.Tags
                .Sort(sortParams)
                .ToPagedAsync(pageParams);

            return Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>.Success(tags);
        }
        catch(Exception) {
            return Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }


    /// <summary> Создать тэг. </summary>
    public async Task<Result> CreateTagAsync( Tag tag, CancellationToken token ) {

        if(tag is null)
            return Result.Error("Аргумент запроса не может быть null");

        bool exist = await _dbContext.Tags.AsNoTracking().AnyAsync(t => t.Name == tag.Name, token);
        if(exist)
            return Result.Error("Тэг с таким именем уже существует");

        try {
            await _dbContext.Tags.AddAsync(tag, token);
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
    public async Task<Result> UpdateTagAsync( Tag tag, CancellationToken token ) {

        if(tag is null)
            return Result.Error("Аргумент запроса не может быть null");

        try {
            _dbContext.Tags.Update(tag);
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


    /// <summary> Удалить тэг.(каскадно) </summary>
    public async Task<Result> DeleteTagAsync( Tag tag, CancellationToken token ) {

        try {
            _dbContext.Tags.Remove(tag);
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
