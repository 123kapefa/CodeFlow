namespace ReputationService.Domain.Policies;

public static class ReputationPoints {

  public const int QuestionUpvote = +10;
  public const int QuestionDownvote = -5;
  public const int AnswerUpvote = +10;
  public const int AnswerDownvote = -5;
  public const int AcceptedAnswer = +15;
  public const int VoterDownvotePenalty = -1;

}