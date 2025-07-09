namespace UserService.Domain.Filters;

public record SortParams (string? OrderBy, SortDirection? SortDirection);