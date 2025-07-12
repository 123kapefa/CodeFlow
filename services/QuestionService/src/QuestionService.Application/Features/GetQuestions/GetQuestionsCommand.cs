using Contracts.Commands;
using QuestionService.Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.GetQuestions;

public record GetQuestionsCommand(PageParams PageParams, SortParams SortParams) : ICommand;
