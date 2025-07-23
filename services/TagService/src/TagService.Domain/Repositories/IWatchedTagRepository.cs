using Ardalis.Result;
using TagService.Domain.Entities;

namespace TagService.Domain.Repositories;

public interface IWatchedTagRepository {
    
    Task<Result<IEnumerable<WatchedTag>>> GetUserWatchedTagsAsync(Guid userId, CancellationToken token);
    Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token);
    Task<Result> DeleteAsync( int tagId, Guid userId, CancellationToken token );
    Task<Result> DeleteUserTagsAsync( IEnumerable<WatchedTag> watchedTags, CancellationToken token );
}
