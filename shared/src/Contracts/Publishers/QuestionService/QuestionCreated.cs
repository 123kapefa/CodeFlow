using Contracts.DTOs.TagService;

namespace Contracts.Publishers.QuestionService;

public record QuestionCreated (List<TagCreateDTO> Tags);