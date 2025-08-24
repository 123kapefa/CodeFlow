using System.Text.Json;

using Abstractions.Commands;

using Ardalis.Result;

using Contracts.Publishers.VoteService;
using Contracts.Responses.VoteService;

using Messaging.Broker;

using VoteService.Domain.Entities;
using VoteService.Domain.Repositories;
using VoteService.Domain.Services;

namespace VoteService.Application.Handlers.SetVote;

public class SetVoteHandler : ICommandHandler<VoteResponse, SetVoteCommand> {

  private readonly IVoteRepository _voteRepository;
  private readonly IAntiAbusePolicy _policy;
  private readonly IMessageBroker _messageBroker;

  public SetVoteHandler (IVoteRepository voteRepository, IAntiAbusePolicy policy, IMessageBroker messageBroker) {
    _voteRepository = voteRepository;
    _policy = policy;
    _messageBroker = messageBroker;
  }

  public async Task<Result<VoteResponse>> Handle (SetVoteCommand command, CancellationToken cancellationToken) {
    if (command.AuthorUserId == command.OwnerUserId)
      return Result.Conflict ("Вы не можете голосовать за свой собственный пост.");

    var policyCheck = await _policy.ValidateAsync(command.AuthorUserId, command.OwnerUserId, command.SourceId, command.SourceType, command.VoteKind, cancellationToken);
    if (policyCheck.IsConflict())
            return Result<VoteResponse>.Conflict(policyCheck.Errors.ToArray());

        var voteResult = await _voteRepository.GetAsync (command.AuthorUserId, command.SourceType, command.SourceId,
      cancellationToken);
    
    if (voteResult.IsNotFound ()) {
      var vote = Vote.Create (command.AuthorUserId, command.SourceId, command.SourceType, command.VoteKind);
      await _voteRepository.AddAsync (vote, cancellationToken);
      await _voteRepository.SaveChangesAsync (cancellationToken);
      await SendMessageAsync (true, vote, command.ParentId, command.OwnerUserId, null, cancellationToken);
      await _voteRepository.SaveChangesAsync (cancellationToken);
      return Result.Success (new VoteResponse (VoteKind.None, command.VoteKind));
    }

    var (oldKind, newKind) = voteResult.Value.Set (command.VoteKind);
    if (oldKind == newKind)
      return Result.Success (new VoteResponse (oldKind, newKind));

    await _voteRepository.UpdateAsync (voteResult.Value, cancellationToken);
    await _voteRepository.SaveChangesAsync (cancellationToken);
    await SendMessageAsync (false, voteResult.Value, command.ParentId, command.OwnerUserId, oldKind, cancellationToken);
    await _voteRepository.SaveChangesAsync (cancellationToken);

    return Result.Success (new VoteResponse (oldKind, newKind));
  }
  
  private async Task SendMessageAsync (bool isNew, Vote vote, Guid parentId, Guid ownerUserId, VoteKind? voteOldKind, CancellationToken ct) {
    
    switch (vote.SourceType) {
      case VotableSourceType.Question: {
        var voteEvent = new QuestionVoted (
          Guid.NewGuid (), parentId, vote.SourceId, ownerUserId, vote.AuthorUserId,voteOldKind ?? VoteKind.None, vote.Kind, vote.Version, vote.ChangedAt);
        await _messageBroker.PublishAsync (voteEvent, ct);
        Console.WriteLine ($"===== Сообщение Question успешно отправлено: {JsonSerializer.Serialize(voteEvent)} =====");
        break;
      }
      case VotableSourceType.Answer: {
        var voteEvent = new AnswerVoted (
          Guid.NewGuid (), parentId, vote.SourceId, ownerUserId, vote.AuthorUserId, voteOldKind ?? VoteKind.None, vote.Kind, vote.Version, vote.ChangedAt);
        await _messageBroker.PublishAsync (voteEvent, ct);
        Console.WriteLine ($"===== Сообщение Answer успешно отправлено: {JsonSerializer.Serialize(voteEvent)} =====");
        break;
      }
    }
  } 

}