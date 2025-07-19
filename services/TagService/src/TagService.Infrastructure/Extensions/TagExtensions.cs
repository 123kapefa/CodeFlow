using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TagService.Domain.Entities;
using TagService.Domain.Filters;

namespace TagService.Infrastructure.Extensions;

public static class TagExtensions {

    public static async Task<Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>> ToPagedAsync(
        this IQueryable<Tag> tags, PageParams pageParams) {

        int count = tags.Count();   

        int page = pageParams.Page ?? 1;
        int pageSize = pageParams.PageSize ?? 36;
        int skip = (page - 1) * pageSize;

        List<Tag> items = tags.Skip(skip).Take(pageSize).ToList();

        int totalPages = (int)Math.Ceiling(count / (double)pageSize);

        PagedInfo pagedInfo = new PagedInfo(page, pageSize, totalPages, count);

        return Result<(IEnumerable<Tag> items, PagedInfo pageInfo)>.Success((items, pagedInfo));
    }

    public static IQueryable<Tag> Sort(this IQueryable<Tag> tags, SortParams sortParams ) {
        return sortParams.SortDirection == SortDirection.Descending ?
            tags = tags.OrderByDescending(GetKey(sortParams.OrderBy)) : tags.OrderBy(GetKey(sortParams.OrderBy));
    }

    private static Expression<Func<Tag, object>> GetKey( string? orderBy ) {
        if(string.IsNullOrEmpty(orderBy))
            return x => x.Name;

        return orderBy switch {
            nameof(Tag.CreatedAt) => x => x.CreatedAt,
            _ => x => x.Name
        };
    }

}
