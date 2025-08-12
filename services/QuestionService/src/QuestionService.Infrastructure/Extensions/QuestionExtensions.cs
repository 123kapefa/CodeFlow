using Ardalis.Result;

using QuestionService.Domain.Entities;

using System.Linq.Expressions;

using Contracts.Common.Filters;

namespace QuestionService.Infrastructure.Extensions;

public static class QuestionExtensions {

  public static async Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> ToPagedAsync (
    this IQueryable<Question> questions
    , PageParams pageParams) {
    int count = questions.Count ();

    int page = pageParams.Page ?? 1;
    int pageSize = pageParams.PageSize ?? 15; // 15 30 50
    int skip = (page - 1) * pageSize;

    List<Question> items = questions.Skip (skip).Take (pageSize).ToList ();
    int totalPages = (int)Math.Ceiling (count / (double)pageSize);

    PagedInfo pagedInfo = new PagedInfo (page, pageSize, totalPages, count);

    return Result<(IEnumerable<Question>, PagedInfo)>.Success ((items, pagedInfo));
  }

  public static IQueryable<Question> Sort (this IQueryable<Question> questions, SortParams sortParams) {
    return sortParams.SortDirection == SortDirection.Descending
      ? questions = questions.OrderByDescending (GetKey (sortParams.OrderBy))
      : questions.OrderBy (GetKey (sortParams.OrderBy));
  }

  private static Expression<Func<Question, object>> GetKey (string? orderBy) {
    if (string.IsNullOrEmpty (orderBy))
      return x => x.AnswersCount;

    return orderBy switch {
      nameof(Question.CreatedAt) => x => x.CreatedAt, _ => x => x.AnswersCount
    };
  }

  public static IQueryable<Question> FilterByTag (this IQueryable<Question> questions, TagFilter tagFilter) {
    if (tagFilter.TagId.HasValue) {
      questions = questions.Where (q => q.QuestionTags.Any (tag => tag.TagId == tagFilter.TagId.Value));
    }

    return questions;
  }

}