namespace Contracts.Common.Filters;

public class PageParams {
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public string ToQueryString() {
        var query = new List<string>();
        if(Page.HasValue)
            query.Add($"page={Page.Value}");
        if(PageSize.HasValue)
            query.Add($"pageSize={PageSize.Value}");
        return string.Join("&", query);
    }
}
