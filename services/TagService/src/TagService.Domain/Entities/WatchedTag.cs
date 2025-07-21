using System.Xml.Linq;

namespace TagService.Domain.Entities;

public class WatchedTag {
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; } 
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;


    protected WatchedTag() { }

    private WatchedTag( Guid userId, int tagId ) {
        UserId = userId;
        TagId = tagId;
    }

    public static WatchedTag Create( Guid userId, int tagId ) {
        return new WatchedTag(userId, tagId);
    }
}
