using Ardalis.Result;
using Microsoft.EntityFrameworkCore.Storage;
using TagService.Domain.Entities;

namespace TagService.Domain.Repositories;

public interface IWatchedTagRepository {
    
    Task<Result<IEnumerable<WatchedTag>>> GetUserWatchedTagsAsync(Guid userId, CancellationToken token);
    Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token);
    Task<Result> DeleteAsync( int tagId, Guid userId, CancellationToken token );
    Task<Result> DeleteUserTagsAsync( IEnumerable<WatchedTag> watchedTags, CancellationToken token );

    Task<bool> ExistsAsync( Guid userId, int tagId, CancellationToken token );
    Task AddAsync( WatchedTag entity, CancellationToken token );   
    Task SaveChangesAsync( CancellationToken token );
    Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken token );   
    Task<List<WatchedTag>> GetUserWatchedTagsListAsync( Guid userId, CancellationToken ct );
    void RemoveRange( IEnumerable<WatchedTag> items );
}
