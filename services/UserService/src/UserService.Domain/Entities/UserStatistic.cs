namespace UserService.Domain.Entities;

public class UserStatistic {
    public Guid Id { get; set; } = Guid.NewGuid ();

    public Guid UserId { get; set; }
    public DateTime? LastVisitAt { get; set; }
    public int VisitCount { get; set; } = 0;
    public int Reputation { get; set; } = 0;
}
