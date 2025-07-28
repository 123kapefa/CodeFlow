using Ardalis.Result;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using TagService.Domain.Entities;
using TagService.Domain.Filters;

namespace TagService.Domain.Repositories;

public interface ITagRepository {

    Task<Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>> GetTagsAsync(
        PageParams pageParams,
        SortParams sortParams,
        CancellationToken token );

    Task<Result<Tag>> GetTagByIdAsync( int tagId, CancellationToken token);
    Task<Result<Tag>> GetTagByNameAsync( string name, CancellationToken token );
    Task<Result> CreateTagAsync(Tag tag, CancellationToken token );
    Task<Result> UpdateTagAsync(Tag tag, CancellationToken token );
    Task<Result> DeleteTagAsync(Tag tag, CancellationToken token );

    Task<Result<List<Tag>>> GetTagsByIdAsync( List<int?> ids, CancellationToken token );
    Task AddRangeAsync( IEnumerable<Tag> tags, CancellationToken token );
    Task SaveChangesAsync( CancellationToken token );
    Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken token );
}
