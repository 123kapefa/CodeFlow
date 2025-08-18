using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Application.Features.VoteChanged;
using ReputationService.Domain.Entities;
using ReputationService.Domain.Policies;

namespace ReputationService.Application.Policies;

public sealed class DefaultReputationPolicy : IReputationPolicy
{
  public (int OwnerNewAmount, ReputationSourceType SourceType, ReasonCode OwnerReason) FromVote(VotableEntityType entityType, VoteKind newKind) {
    var st = entityType == VotableEntityType.Answer ? ReputationSourceType.Answer : ReputationSourceType.Question;
    int owner = (st, newKind) switch {
      (ReputationSourceType.Answer,   VoteKind.Up)   => ReputationPoints.AnswerUpvote,
      (ReputationSourceType.Answer,   VoteKind.Down) => ReputationPoints.AnswerDownvote,
      (ReputationSourceType.Question, VoteKind.Up)   => ReputationPoints.QuestionUpvote,
      (ReputationSourceType.Question, VoteKind.Down) => ReputationPoints.QuestionDownvote,
      _ => 0
    };

    var reason = newKind == VoteKind.Up ? ReasonCode.Upvote :
      newKind == VoteKind.Down ? ReasonCode.Downvote : ReasonCode.Upvote;
    return (owner, st, reason);
  }

  public (int OldOwnerNewAmount, int NewOwnerNewAmount) FromAcceptedAnswer()
    => (0, ReputationPoints.AcceptedAnswer);
}
