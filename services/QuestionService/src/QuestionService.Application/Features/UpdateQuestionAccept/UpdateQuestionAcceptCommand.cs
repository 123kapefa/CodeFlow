using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.UpdateQuestionAccept;

public record UpdateQuestionAcceptCommand( Guid QuestionId, Guid AcceptedAnswerId ) : ICommand;
