namespace Contracts.DTOs.TagService;

public class WatchedTagDTO {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }   
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;

    public WatchedTagDTO() { }

    private WatchedTagDTO( Guid id, Guid userId, int tagId, string tagName ) {
        Id = id;
        UserId = userId;
        TagId = tagId;
        TagName = tagName;
    }

    public static WatchedTagDTO Create( Guid id, Guid userId, int tagId, string tagName ) {
        return new WatchedTagDTO(id, userId, tagId, tagName);
    }
}
