using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Policies;

public interface IReputationPolicy {
  IReadOnlyList<ReputationEntry> FromVoteChanged(VoteChanged e);
  IReadOnlyList<ReputationEntry> FromAcceptedAnswerChanged(AnswerAccepted e);
}