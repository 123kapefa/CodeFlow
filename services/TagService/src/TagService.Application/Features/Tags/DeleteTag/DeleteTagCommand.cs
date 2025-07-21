using Contracts.Commands;

namespace TagService.Application.Features.Tags.DeleteTag;

public record DeleteTagCommand(int TagId) : ICommand;
