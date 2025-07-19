using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagService.Application.DTO;

namespace TagService.Application.Features.Tags.UpdateTag;

public record UpdateTagCommand(int tagId, TagUpdateDTO TagUpdateDTO) : ICommand;
