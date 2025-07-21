using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Contracts.Commands;
using FluentValidation;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.CreateTag;

public class CreateTagHandler : ICommandHandler<CreateTagCommand> {

    private readonly ITagRepository _tagRepository;
    private readonly IValidator<CreateTagCommand> _validator;

    public CreateTagHandler( ITagRepository tagRepository, IValidator<CreateTagCommand> validator ) {
        _tagRepository = tagRepository;
        _validator = validator;
    }


    public async Task<Result> Handle( CreateTagCommand command, CancellationToken token ) {

        var validated = await _validator.ValidateAsync(command);
        if(!validated.IsValid)
            return Result.Invalid(validated.AsErrors());

        //Tag tag1 = new Tag {
        //    Name = command.TagCreateDto.Name,
        //    Description = command.TagCreateDto.Description,
        //    CreatedAt = DateTime.UtcNow
        //};

        Tag tag = Tag.Create(command.TagCreateDto.Name, command.TagCreateDto.Description);

        Result result = await _tagRepository.CreateTagAsync(tag, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
