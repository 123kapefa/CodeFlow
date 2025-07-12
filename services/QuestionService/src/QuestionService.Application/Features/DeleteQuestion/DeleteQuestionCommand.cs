using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.DeleteQuestion;

public record DeleteQuestionCommand (Guid QuestionId) : ICommand;
