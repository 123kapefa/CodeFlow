using Abstractions.Commands;

using Contracts.DTOs.TagService;
using Contracts.Requests.TagService;

namespace TagService.Application.Features.Tags.CreateTags;

public record CreateTagsCommand (EnsureTagsRequest Request) : ICommand;