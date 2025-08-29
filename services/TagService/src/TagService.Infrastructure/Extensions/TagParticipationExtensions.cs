using System.Linq.Expressions;

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
        return sortParams.SortDirection == SortDirection.Descending 
            ? tags = tags.OrderByDescending(GetKey (sortParams.OrderBy)) 
            : tags.OrderBy(GetKey (sortParams.OrderBy));
    }    
    
    private static Expression<Func<UserTagParticipation, object>> GetKey (string? orderBy) {
        if (string.IsNullOrEmpty (orderBy))
            return x => x.Tag.Name;

        return orderBy switch {
            nameof(UserTagParticipation.QuestionsCreated) => x => x.QuestionsCreated, 
            nameof(UserTagParticipation.AnswersWritten) => x => x.AnswersWritten, 
            _ => x => x.Tag.Name
        };
    }

}
