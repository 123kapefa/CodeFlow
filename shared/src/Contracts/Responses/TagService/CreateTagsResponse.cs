using Contracts.DTOs.TagService;

namespace Contracts.Responses.TagService;

public record CreateTagsResponse (List<CreateTagDto> CreatedTags);