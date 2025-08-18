using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Policies;

public static class ReputationReasonText {

  private static readonly Dictionary<(ReputationSourceType, ReasonCode), string> Map = new () {
    [(ReputationSourceType.Question, ReasonCode.Upvote)] = "Upvote Question",
    [(ReputationSourceType.Question, ReasonCode.Downvote)] = "Downvote Question",
    [(ReputationSourceType.Answer, ReasonCode.Upvote)] = "Upvote Answer",
    [(ReputationSourceType.Answer, ReasonCode.Downvote)] = "Downvote Answer",
    [(ReputationSourceType.Answer, ReasonCode.AcceptedAnswer)] = "Accepted Answer",
    [(ReputationSourceType.Answer, ReasonCode.AcceptedAnswer)] = "Accepted Answer",
    [(ReputationSourceType.Answer, ReasonCode.DeleteAnswer)] = "Delete Answer",
  };

  public static string For (ReputationSourceType st, ReasonCode rc) =>
    Map.TryGetValue ((st, rc), out var text) ? text : $"{rc} {st}";

}