using Contracts.Commands;
using QuestionService.Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.GetUserQuestions;
public record GetUserQuestionsCommand( Guid UserId, PageParams PageParams, SortParams SortParams )  : ICommand;
