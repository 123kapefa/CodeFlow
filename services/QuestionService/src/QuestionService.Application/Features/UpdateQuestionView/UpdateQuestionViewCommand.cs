using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.UpdateQuestionView;

public record UpdateQuestionViewCommand (Guid QuestionId) : ICommand;