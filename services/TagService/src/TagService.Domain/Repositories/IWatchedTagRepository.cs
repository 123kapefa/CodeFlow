using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagService.Domain.Entities;

namespace TagService.Domain.Repositories;

public interface IWatchedTagRepository {

    Task<Result<IEnumerable<WatchedTag>>> GetUserWatchedTagsAsync(Guid userId, CancellationToken token);
    Task<Result> CreateAsync( WatchedTag watchedTag, CancellationToken token);
    Task<Result> DeleteAsync( Guid userId, int tagId, CancellationToken token );
}
