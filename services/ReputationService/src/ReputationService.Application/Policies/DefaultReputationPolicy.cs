using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Application.Features.VoteChanged;
using ReputationService.Domain.Entities;
using ReputationService.Domain.Policies;

namespace ReputationService.Application.Policies;

public sealed class DefaultReputationPolicy : IReputationPolicy
{
  public (int Delta, ReputationSourceType SourceType, ReasonCode OwnerReason) FromVote(VotableSourceType sourceType, VoteKind oldKind, VoteKind newKind) {
    var st = sourceType == VotableSourceType.Answer ? ReputationSourceType.Answer : ReputationSourceType.Question;
    
    int delta = (sourceType, oldKind, newKind) switch {
      (VotableSourceType.Question, VoteKind.None, VoteKind.Up) => +10,
      (VotableSourceType.Question, VoteKind.None, VoteKind.Down) => -5,
      (VotableSourceType.Question, VoteKind.Up, VoteKind.None) => -10,
      (VotableSourceType.Question, VoteKind.Up, VoteKind.Down) => -15,
      (VotableSourceType.Question, VoteKind.Down, VoteKind.None) => +5,
      (VotableSourceType.Question, VoteKind.Down, VoteKind.Up) => +15,
      (VotableSourceType.Answer, VoteKind.None, VoteKind.Up) => +10,
      (VotableSourceType.Answer, VoteKind.None, VoteKind.Down) => -5,
      (VotableSourceType.Answer, VoteKind.Up, VoteKind.None) => -10,
      (VotableSourceType.Answer, VoteKind.Up, VoteKind.Down) => -15,
      (VotableSourceType.Answer, VoteKind.Down, VoteKind.None) => +5,
      (VotableSourceType.Answer, VoteKind.Down, VoteKind.Up) => +15,
      _ => 0
    };

    var reason = newKind switch {
      VoteKind.Up => ReasonCode.Upvote,
      VoteKind.Down => ReasonCode.Downvote,
      _ => ReasonCode.Upvote
    };
    return (delta, st, reason);
  }

  public (int OldDelta, int NewDelta, ReasonCode Reason) FromAcceptedAnswerChange()
    => (-ReputationPoints.AcceptedAnswer, +ReputationPoints.AcceptedAnswer, ReasonCode.AcceptedAnswer);
}
