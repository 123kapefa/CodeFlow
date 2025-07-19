using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagService.Domain.Filters;

namespace TagService.Application.Features.Tags.GetTags;

public record GetTagsCommand( PageParams PageParams, SortParams SortParams ) : ICommand;
