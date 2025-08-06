using Abstractions.Commands;

using Contracts.DTOs.TagService;

namespace TagService.Application.Features.Tags.CreateTags;

public record CreateTagsCommand (List<CreateTagDto> QuestionTags) : ICommand;