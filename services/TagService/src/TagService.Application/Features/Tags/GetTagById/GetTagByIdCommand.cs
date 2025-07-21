using Abstractions.Commands;

namespace TagService.Application.Features.Tags.GetTagById;

public record GetTagByIdCommand( int TagId ) : ICommand;
