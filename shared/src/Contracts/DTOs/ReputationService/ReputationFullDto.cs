using System.Text.Json.Serialization;

namespace Contracts.DTOs.ReputationService;

public class ReputationFullDto {
  
  public Guid ParentId { get; set; }
  public string? Title { get; set; }
  public Guid SourceId { get; set; }
  public string SourceType { get; set; }
  public string ReasonCode { get; set; }
  public int Delta { get; set; }
  public DateTime OccurredAt { get; set; }

  public ReputationFullDto() { }
  
  private ReputationFullDto(Guid parentId, Guid sourceId, string sourceType, 
    string reasonCode, int delta, DateTime occurredAt) {
    ParentId = parentId;
    SourceId = sourceId;
    SourceType = sourceType;
    ReasonCode = reasonCode;
    Delta = delta;
    OccurredAt = occurredAt;
  }

  public static ReputationFullDto Create (
    Guid parentId,
    Guid sourceId,
    string sourceType,
    string reasonCode,
    int amount,
    DateTime occurredAt) =>
    new ReputationFullDto (parentId, sourceId, sourceType, 
      reasonCode, amount, occurredAt);

}