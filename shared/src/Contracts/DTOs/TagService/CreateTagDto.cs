namespace Contracts.DTOs.TagService;

public class CreateTagDto {
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
