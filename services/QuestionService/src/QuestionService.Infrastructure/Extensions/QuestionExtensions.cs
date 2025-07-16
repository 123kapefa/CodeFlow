using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Infrastructure.Extensions;

public static class QuestionExtensions {

    public static async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> ToPagedAsync( this IQueryable<Question> questions, PageParams pageParams ) {

        int count = questions.Count();

        int page = pageParams.Page ?? 1;
        int pageSize = pageParams.PageSize ?? 50; // 15 30 50
        int skip = (page - 1) * pageSize;

        List<Question> items = questions.Skip(skip).Take(pageSize).ToList();
        int totalPages = (int)Math.Ceiling(count / (double)pageSize);

        PagedInfo pagedInfo = new PagedInfo(page, pageSize, totalPages, count);

        return Result<(IEnumerable<Question>, PagedInfo)>.Success((items, pagedInfo));
    }

    public static IQueryable<Question> Sort( this IQueryable<Question> questions, SortParams sortParams ) {

        return sortParams.SortDirection == SortDirection.Descending ?
            questions = questions.OrderByDescending(GetKey(sortParams.OrderBy)) : questions.OrderBy(GetKey(sortParams.OrderBy));
    }

    private static Expression<Func<Question, object>> GetKey( string? orderBy ) {
        if(string.IsNullOrEmpty(orderBy))
            return x => x.AnswersCount;

        return orderBy switch {
            nameof(Question.CreatedAt) => x => x.CreatedAt,
            _ => x => x.AnswersCount
        };
    }

}
