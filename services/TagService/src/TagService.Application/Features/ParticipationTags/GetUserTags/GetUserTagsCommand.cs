using Contracts.Commands;
using TagService.Domain.Filters;

namespace TagService.Application.Features.ParticipationTags.GetUserTags;

public record GetUserTagsCommand( Guid UserId, PageParams PageParams, SortParams SortParams ) : ICommand;
