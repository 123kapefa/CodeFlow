using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Data;

namespace TagService.Infrastructure.Repositories;

public class WatchedTagRepository : IWatchedTagRepository {

    private readonly TagServiceDbContext _dbContext;

    public WatchedTagRepository( TagServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получить тэг по ID. </summary>
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


    /// <summary> Получить тэг по ID. </summary>
    public async Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token ) {

        if(watchedTag is null)
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


    /// <summary> Получить тэг по ID. </summary>
    public Task<Result> DeleteAsync( Guid userId, int tagId, CancellationToken token ) => throw new NotImplementedException();   
}
