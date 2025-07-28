using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Filters;

namespace TagService.Domain.Repositories;

public interface IUserTagParticipationRepository {

    Task<Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>> GetTagsAsync(
       Guid userId,
       PageParams pageParams,
       SortParams sortParams,
       CancellationToken token );

    Task<Result<UserTagParticipation?>> GetUserTagParticipation(Guid userId, int tagId, CancellationToken token);
    Task<Result> CreateAsync( UserTagParticipation tagParticipation, CancellationToken token );
    Task<Result> UpdateAsync( UserTagParticipation tagParticipation, CancellationToken token );
    Task<Result> DeleteUserTagsAsync(Guid userId, CancellationToken token);

    Task<Dictionary<int, UserTagParticipation>> GetByUserAndTagIdsAsync(
        Guid userId, IEnumerable<int> tagIds, CancellationToken ct );
    Task AddRangeAsync( IEnumerable<UserTagParticipation> items, CancellationToken token );
    Task AddQuestionsRangeAsync( IEnumerable<UserTagParticipationQuestion> items, CancellationToken ct );
}
