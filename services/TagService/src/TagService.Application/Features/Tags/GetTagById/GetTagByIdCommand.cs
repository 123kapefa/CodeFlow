using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.Features.Tags.GetTagById;

public record GetTagByIdCommand( int TagId ) : ICommand;
