using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Application.Features.VoteChanged;
using ReputationService.Domain.Entities;
using ReputationService.Domain.Policies;

namespace ReputationService.Application.Policies;

public sealed class DefaultReputationPolicy {

  public IReadOnlyList<ReputationEntry> FromVoteChanged (VoteChangedCommand command) {
    var entries = new List<ReputationEntry> ();
    
    var ownerDelta = PointsFor (command.EntityType, command.NewKind) - PointsFor (command.EntityType, command.OldKind);
    if (ownerDelta != 0) {
      entries.Add (ReputationEntry.Create (
        userId: command.EntityOwnerUserId, 
        sourceId: command.EntityId,
        sourceType: ToSourceType (command.EntityType),
        delta: ownerDelta,
        reasonCode: ownerDelta > 0 ? ReasonCode.Upvote : ReasonCode.Downvote, 
        occurredAt: command.OccurredAt,
        sourceEventId: command.SourceEventId,
        sourceService: "VoteService"
        )
      );
    }
    
    var voterDelta = Penalty (command.NewKind) - Penalty (command.OldKind);
    if (voterDelta != 0) {
      entries.Add (ReputationEntry.Create (
        userId: command.ActorUserId, 
        sourceId: command.EntityId,
        sourceType: ToSourceType (command.EntityType), 
        delta: voterDelta, 
        reasonCode: ReasonCode.Downvote,
        occurredAt: command.OccurredAt,
        sourceEventId: command.SourceEventId,
        sourceService: "VoteService"
        )
      );
    }

    return entries;
  }

  public IReadOnlyList<ReputationEntry> FromAcceptedAnswerChanged (AnswerAccepted e) {
    var entries = new List<ReputationEntry> ();
    const int p = ReputationPoints.AcceptedAnswer;

    if (e.OldAnswerOwnerUserId is Guid oldOwner)
      entries.Add (ReputationEntry.Create (oldOwner, e.QuestionId, ReputationSourceType.Answer, -p,
        ReasonCode.AcceptedAnswer, e.OccurredAt, sourceEventId: e.EventId,
        sourceService: "VoteService"));

    if (e.NewAnswerOwnerUserId is Guid newOwner)
      entries.Add (ReputationEntry.Create (newOwner, e.QuestionId, ReputationSourceType.Answer, +p,
        ReasonCode.AcceptedAnswer, e.OccurredAt, sourceEventId: e.EventId,
        sourceService: "VoteService"));

    return entries;
  }

  private static int PointsFor (VotableEntityType t, VoteKind k) =>
    (t, k) switch {
      (VotableEntityType.Answer, VoteKind.Up) => ReputationPoints.AnswerUpvote,
      (VotableEntityType.Answer, VoteKind.Down) => ReputationPoints.AnswerDownvote,
      (VotableEntityType.Question, VoteKind.Up) => ReputationPoints.QuestionUpvote,
      (VotableEntityType.Question, VoteKind.Down) => ReputationPoints.QuestionDownvote,
      _ => 0
    };

  private static int Penalty (VoteKind k) => k == VoteKind.Down ? ReputationPoints.VoterDownvotePenalty : 0;

  private static ReputationSourceType ToSourceType (VotableEntityType t) =>
    t == VotableEntityType.Answer ? ReputationSourceType.Answer : ReputationSourceType.Question;

}