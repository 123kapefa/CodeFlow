namespace TagService.Domain.Entities;

public class UserTagParticipationQuestion {
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public Guid UserTagParticipationId { get; set; }
    public UserTagParticipation UserTagParticipation { get; set; } = null!;


    protected UserTagParticipationQuestion() { }

    private UserTagParticipationQuestion( Guid questionId ) {
        QuestionId = questionId;
    }

    public static UserTagParticipationQuestion Create( Guid questionId ) {
        return new UserTagParticipationQuestion(questionId);
    }
}
