namespace Contracts.DTOs.ReputationService;

public class ReputationGroupedByDayDto {
  
  public DateOnly Date { get; init; }
  public int Delta { get; init; }
  public List<ReputationFullDto> Events { get; init; } = new();

}