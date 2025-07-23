using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionVote;

public record UpdateQuestionVoteCommand(Guid QuestionId, int VoteValue) : ICommand;
