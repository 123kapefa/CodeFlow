using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.Features.Tags.UpdateTagWatchers;

public record UpdateTagWatchersCommand(int TagId, int Count) : ICommand;
