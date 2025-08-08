namespace Contracts.Common.Filters;

public class TagFilter {
    public int? TagId { get; set; }

    public string ToQueryString() {
        return TagId.HasValue ? $"tagId={TagId.Value}" : "";
    }
}