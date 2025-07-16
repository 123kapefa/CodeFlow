using Contracts.Commands;
using QuestionService.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.CreateQuestion;

public record CreateQuestionCommand (CreateQuestionDTO CreateQuestionDTO) : ICommand;