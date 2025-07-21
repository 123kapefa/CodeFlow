namespace TagService.Application.DTO;

public class CreateParticipationDto {
    public Guid UserId { get; set; }
    public int TagId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsAnswer { get; set; } = false;
}
