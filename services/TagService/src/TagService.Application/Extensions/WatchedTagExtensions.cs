using Contracts.DTOs.TagService;

using TagService.Domain.Entities;

namespace TagService.Application.Extensions;

public static class WatchedTagExtensions {

  public static WatchedTagDTO ToWatchedTagDto (this WatchedTag watchedTag) => new WatchedTagDTO {
    Id = watchedTag.Id,
    UserId = watchedTag.UserId,
    TagId = watchedTag.TagId,
    TagName = watchedTag.Tag.Name
  };

  public static IEnumerable<WatchedTagDTO> ToWatchedTagsDto (this IEnumerable<WatchedTag> watchedTags) =>
    watchedTags.Select (wt => new WatchedTagDTO {
    Id = wt.Id,
    UserId = wt.UserId,
    TagId = wt.TagId,
    TagName = wt.Tag.Name
  });

}