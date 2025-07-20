namespace TagService.Domain.Entities;

public class UserTagParticipationQuestion {
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public Guid UserTagParticipationId { get; set; }
    public UserTagParticipation UserTagParticipation { get; set; } = null!;
}
