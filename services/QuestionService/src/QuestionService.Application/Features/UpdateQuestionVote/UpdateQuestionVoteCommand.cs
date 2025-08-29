using Abstractions.Commands;

using Contracts.Publishers.VoteService;

namespace QuestionService.Application.Features.UpdateQuestionVote;

public record UpdateQuestionVoteCommand (Guid QuestionId, Guid AuthorUserId, VoteKind VoteValue) : ICommand;