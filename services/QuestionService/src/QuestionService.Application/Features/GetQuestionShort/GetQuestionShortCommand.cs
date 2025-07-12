using Contracts.Commands;
using QuestionService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.GetQuestionShort;
public record GetQuestionShortCommand(Guid questionId) : ICommand;
