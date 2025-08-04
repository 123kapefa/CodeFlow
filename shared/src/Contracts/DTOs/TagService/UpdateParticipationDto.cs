namespace Contracts.DTOs.TagService;

public class UpdateParticipationDto {
    public Guid UserId { get; set; }
    public int TagId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsAnswer { get; set; } = false;
}
