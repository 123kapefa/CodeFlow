using System.Linq.Expressions;

using Ardalis.Result;

using Contracts.Common.Filters;

using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Extensions;

public static class ReputationExtensions {

  public static async Task<Result<(IEnumerable<ReputationEntry> items, PagedInfo pageInfo)>> ToPagedAsync (
    this IQueryable<ReputationEntry> reputationEntries
    , PageParams pageParams) {
    int count = reputationEntries.Count ();

    int page = pageParams.Page ?? 1;
    int pageSize = pageParams.PageSize ?? 30;
    int skip = (page - 1) * pageSize;

    List<ReputationEntry> items = reputationEntries.Skip (skip).Take (pageSize).ToList ();
    int totalPages = (int)Math.Ceiling (count / (double)pageSize);

    PagedInfo pagedInfo = new PagedInfo (page, pageSize, totalPages, count);

    return Result<(IEnumerable<ReputationEntry>, PagedInfo)>.Success ((items, pagedInfo));
  }

  public static IQueryable<ReputationEntry> Sort (this IQueryable<ReputationEntry> answers, SortParams sortParams) {
    return sortParams.SortDirection == SortDirection.Descending
      ? answers = answers.OrderByDescending (GetKey (sortParams.OrderBy))
      : answers.OrderBy (GetKey (sortParams.OrderBy));
  }

  private static Expression<Func<ReputationEntry, object>> GetKey (string? orderBy) {
    if (string.IsNullOrEmpty (orderBy))
      return x => x.OccurredAt;

    return orderBy switch {
      nameof(ReputationEntry.OccurredAt) => x => x.OccurredAt,
      _ => x => x.OccurredAt
    };
  }

}