using Contracts.DTOs.TagService;

using TagService.Domain.Entities;

namespace TagService.Application.Extensions;

public static class TagExtensions {

  public static TagDTO ToTagDto ( this Tag tag ) =>
    TagDTO.Create(
      tag.Id,
      tag.Name,
      tag.Description,
      tag.CreatedAt,
      tag.CountQuestion,
      tag.CountWotchers,
      tag.DailyRequestCount,
      tag.WeeklyRequestCount
    );
  
  public static IEnumerable<TagDTO> ToTagsDto ( this IEnumerable<Tag> tags ) =>
    tags.Select(tag => TagDTO.Create(
      tag.Id,
      tag.Name,
      tag.Description,
      tag.CreatedAt,
      tag.CountQuestion,
      tag.CountWotchers,
      tag.DailyRequestCount,
      tag.WeeklyRequestCount
    ));

}