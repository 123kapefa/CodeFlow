using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Filters;

namespace TagService.Domain.Repositories;

public interface IUserTagParticipationRepository {

    Task<Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>> GetTagsAsync(
       PageParams pageParams,
       SortParams sortParams,
       CancellationToken token );

    Task<Result> CreateAsync( UserTagParticipation tagParticipation, CancellationToken token );
    Task<Result> UpdateAsync( UserTagParticipation tagParticipation, CancellationToken token );
    Task<Result> DeleteUserTagsAsync(Guid userId, CancellationToken token);
}
