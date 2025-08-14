using System.Linq.Expressions;

using AnswerService.Domain.Entities;

using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.DTOs.AnswerService;

namespace AnswerService.Infrastructure.Extensions;

public static class AnswerExtensions {
  
  public static async Task<Result<(IEnumerable<Answer> items, PagedInfo pageInfo)>> ToPagedAsync (
    this IQueryable<Answer> answers
    , PageParams pageParams) {
    int count = answers.Count ();

    int page = pageParams.Page ?? 1;
    int pageSize = pageParams.PageSize ?? 30;
    int skip = (page - 1) * pageSize;

    List<Answer> items = answers.Skip (skip).Take (pageSize).ToList ();
    int totalPages = (int)Math.Ceiling (count / (double)pageSize);

    PagedInfo pagedInfo = new PagedInfo (page, pageSize, totalPages, count);

    return Result<(IEnumerable<Answer>, PagedInfo)>.Success ((items, pagedInfo));
  }

  public static IQueryable<Answer> Sort (this IQueryable<Answer> answers, SortParams sortParams) {
    return sortParams.SortDirection == SortDirection.Descending
      ? answers = answers.OrderByDescending (GetKey (sortParams.OrderBy))
      : answers.OrderBy (GetKey (sortParams.OrderBy));
  }

  private static Expression<Func<Answer, object>> GetKey (string? orderBy) {
    if (string.IsNullOrEmpty (orderBy))
      return x => x.CreatedAt;

    return orderBy switch {
      nameof(Answer.CreatedAt) => x => x.CreatedAt,
      _ => x => x.CreatedAt
    };
  }

}