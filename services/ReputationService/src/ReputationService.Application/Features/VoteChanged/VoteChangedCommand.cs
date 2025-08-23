using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace ReputationService.Application.Features.VoteChanged;

public record VoteChangedCommand (
      Guid SourceEventId,
      Guid ParentId,
      Guid SourceId, 
      Guid OwnerUserId, 
      Guid AuthorUserId,
      string SourceService,
      VotableSourceType SourceType,
      VoteKind OldKind,
      VoteKind NewKind,
      int  Version,
      DateTime OccurredAt) : ICommand;