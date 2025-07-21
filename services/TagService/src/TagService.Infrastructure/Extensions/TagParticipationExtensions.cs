using Ardalis.Result;
using TagService.Domain.Entities;
using TagService.Domain.Filters;

namespace TagService.Infrastructure.Extensions;

public static class TagParticipationExtensions {

    public static async Task<Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>> ToPagedAsync(
       this IQueryable<UserTagParticipation> tags, PageParams pageParams ) {

        int count = tags.Count();

        int page = pageParams.Page ?? 1;
        int pageSize = pageParams.PageSize ?? 50;
        int skip = (page - 1) * pageSize;

        List<UserTagParticipation> items = tags.Skip(skip).Take(pageSize).ToList();

        int totalPages = (int)Math.Ceiling(count / (double)pageSize);

        PagedInfo pagedInfo = new PagedInfo(page, pageSize, totalPages, count);

        return Result<(IEnumerable<UserTagParticipation> items, PagedInfo pageInfo)>.Success((items, pagedInfo));
    }


    public static IQueryable<UserTagParticipation> Sort( this IQueryable<UserTagParticipation> tags, SortParams sortParams ) {
        return sortParams.SortDirection == SortDirection.Descending ?
            tags = tags.OrderByDescending(t => t.Tag.Name) : tags.OrderBy(t => t.Tag.Name);
    }    

}
