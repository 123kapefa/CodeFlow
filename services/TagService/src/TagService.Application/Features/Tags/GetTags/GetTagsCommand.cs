using Abstractions.Commands;

using Contracts.Common.Filters;

using PageParams = TagService.Domain.Filters.PageParams;
using SortParams = TagService.Domain.Filters.SortParams;

namespace TagService.Application.Features.Tags.GetTags;

public record GetTagsCommand( PageParams PageParams, SortParams SortParams, SearchFilter SearchFilter ) : ICommand;
