namespace Contracts.Common.Filters;

public class SortParams {
    public string? OrderBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    public string ToQueryString() {
        var query = new List<string>();
        if(!string.IsNullOrEmpty(OrderBy))
            query.Add($"orderBy={OrderBy}");
        // Приводим enum к int:
        query.Add($"sortDirection={(int)SortDirection}");
        return string.Join("&", query);
    }
}