using Contracts.Commands;
using TagService.Domain.Filters;

namespace TagService.Application.Features.Tags.GetTags;

public record GetTagsCommand( PageParams PageParams, SortParams SortParams ) : ICommand;
