using Contracts.Commands;
using TagService.Application.DTO;

namespace TagService.Application.Features.Tags.CreateTag;

public record CreateTagCommand( TagCreateDTO TagCreateDto ) : ICommand;
