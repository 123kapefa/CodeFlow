using Abstractions.Commands;

using Contracts.DTOs.TagService;

namespace TagService.Application.Features.Tags.CreateTag;

public record CreateTagCommand( TagCreateDTO TagCreateDto ) : ICommand;
