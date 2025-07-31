using Contracts.QuestionService.DTOs;
using Contracts.TagService;

namespace Contracts.Publishers.QuestionService;

public record QuestionCreated (List<TagCreateDTO> Tags);