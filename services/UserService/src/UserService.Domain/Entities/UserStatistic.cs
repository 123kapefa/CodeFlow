namespace UserService.Domain.Entities;

public class UserStatistic {
    public Guid Id { get; set; } = Guid.NewGuid ();

    public Guid UserId { get; set; }
    public DateTime LastVisitAt { get; set; } = DateTime.UtcNow;
    public int VisitCount { get; set; } = 0;
    public int Reputation { get; set; } = 0;

    public UserInfo UserInfo { get; set; } = null!;

    protected UserStatistic() { }

    private UserStatistic(Guid userId) {
        UserId = userId;
        VisitCount += 1;
    }

    public static UserStatistic Create(Guid userId) => new UserStatistic(userId);  
    
    public UserStatistic UpdateReputation(int reputation) {
        Reputation = reputation;
        return this;
    }

}
