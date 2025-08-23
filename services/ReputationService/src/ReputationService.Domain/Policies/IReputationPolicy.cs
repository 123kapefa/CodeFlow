using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Policies;


public interface IReputationPolicy
{
  /// Превращает VoteChanged в целевые Amount-ы для 2 эффектов
  (int Delta, ReputationSourceType SourceType, ReasonCode OwnerReason) FromVote(VotableSourceType entityType, VoteKind oldKind, VoteKind newKind);

  /// Принятый ответ -> два эффекта по разным пользователям
  (int OldDelta, int NewDelta, ReasonCode Reason) FromAcceptedAnswerChange ();

}