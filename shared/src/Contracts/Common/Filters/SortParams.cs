namespace Contracts.Common.Filters;

public record SortParams (string? OrderBy, SortDirection? SortDirection) {

  public string ToQueryString() {
    var parts = new System.Collections.Generic.List<string>(2);

    if (!string.IsNullOrWhiteSpace(OrderBy)) {
      parts.Add($"OrderBy={System.Uri.EscapeDataString(OrderBy)}");
    }

    if (SortDirection.HasValue) {
      parts.Add($"SortDirection={System.Uri.EscapeDataString(SortDirection.Value.ToString())}");
    }
    
    return string.Join("&", parts);
  }

}
