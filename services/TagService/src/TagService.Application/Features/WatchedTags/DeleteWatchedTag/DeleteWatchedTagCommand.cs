using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.Features.WatchedTags.DeleteWatchedTag;

public record DeleteWatchedTagCommand(int TagId, Guid UserId) : ICommand;
