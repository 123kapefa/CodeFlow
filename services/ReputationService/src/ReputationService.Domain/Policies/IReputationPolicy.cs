using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using ReputationService.Domain.Entities;

namespace ReputationService.Domain.Policies;


public interface IReputationPolicy
{
  /// Превращает VoteChanged в целевые Amount-ы для 2 эффектов
  (int OwnerNewAmount, ReputationSourceType SourceType, ReasonCode OwnerReason) FromVote(VotableEntityType entityType, VoteKind newKind);
  /// Принятый ответ -> два эффекта по разным пользователям
  (int OldOwnerNewAmount, int NewOwnerNewAmount) FromAcceptedAnswer();
}