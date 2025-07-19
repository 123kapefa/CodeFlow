using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.Features.Tags.UpdateTagRequest;

//TODO ПОДУМАТЬ НА СЧЕТ ПАРАМЕТРОВ ... МОЖЕТ int tagID???? ... МОЖЕТ List<string> names???
public record UpdateTagRequestCommand(string Name) : ICommand;
