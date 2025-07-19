using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;

//TODO УДАЛИТЬ?? ИСПОЛЬЗОВАТЬ UpdateTagRequestCommand???
public record UpdateTagCountQuestionCommand(string Name, int Count): ICommand;
