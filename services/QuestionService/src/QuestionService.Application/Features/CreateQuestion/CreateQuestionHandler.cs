using Ardalis.Result;
using Contracts.Commands;
using FluentValidation;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.CreateQuestion;

public class CreateQuestionHandler : ICommandHandler<CreateQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;
    private readonly IValidator<CreateQuestionCommand> _validator;

    public CreateQuestionHandler( IQuestionServiceRepository questionServiceRepository, IValidator<CreateQuestionCommand> validator ) {
        _questionServiceRepository = questionServiceRepository;
        _validator = validator;
    }

    public async Task<Result> Handle( CreateQuestionCommand command, CancellationToken cancellationToken ) {

        var validationResult = await _validator.ValidateAsync(command);

        Question question = new Question {
            UserId = command.QuestionDTO.UserId,
            Title = command.QuestionDTO.Title,
            Content = command.QuestionDTO.Content,
            CreatedAt = DateTime.UtcNow,
            QuestionTags = command.QuestionDTO.QuestionTags
        };

    }

}
