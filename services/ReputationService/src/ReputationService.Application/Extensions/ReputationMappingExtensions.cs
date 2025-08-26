using Contracts.DTOs.ReputationService;

using ReputationService.Domain.Entities;

namespace ReputationService.Application.Extensions;

public static class ReputationMappingExtensions {

  public static ReputationShortDto ToReputationShortDto(this IGrouping<DateTime, ReputationEntry> group)
    => ReputationShortDto.Create (
      DateOnly.FromDateTime(group.Key),
      
      group.Sum(x => x.Delta)
    );

  public static IReadOnlyList<ReputationShortDto> ToReputationShortDtoList(this IEnumerable<IGrouping<DateTime, ReputationEntry>> groups)
    => groups.Select(g => g.ToReputationShortDto()).ToList();
  
  public static ReputationGroupedByDayDto ToGroupedDto(this IGrouping<DateOnly, ReputationEntry> group)
    => new ReputationGroupedByDayDto {
      Date = group.Key,
      Delta = group.Sum(x => x.Delta),
      Events = group
       .OrderByDescending(x => x.OccurredAt)
       .Select(x => ReputationFullDto.Create(
          x.ParentId,
          x.SourceId,
          x.SourceType.ToString(),
          x.ReasonCode.ToString(),
          x.Delta,
          x.OccurredAt
        )).ToList()
    };

  public static IReadOnlyList<ReputationGroupedByDayDto> ToGroupedDtoList(
    this IEnumerable<IGrouping<DateOnly, ReputationEntry>> groups)
    => groups.Select(g => g.ToGroupedDto()).ToList();

  
}