using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Contracts.Commands;
using FluentValidation;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.UpdateTag;

public class UpdateTagHandler : ICommandHandler<UpdateTagCommand> {

    private readonly ITagRepository _tagRepository;
    private readonly IValidator<UpdateTagCommand> _validator;

    public UpdateTagHandler( ITagRepository tagRepository, IValidator<UpdateTagCommand> validator ) {
        _tagRepository = tagRepository;
        _validator = validator;
    }

    public async Task<Result> Handle( UpdateTagCommand command, CancellationToken token ) {

        var validated = await _validator.ValidateAsync(command);
        if(!validated.IsValid)
            return Result.Invalid(validated.AsErrors());

        Result<Tag> resultTag = await _tagRepository.GetTagByIdAsync(command.tagId, token);

        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        resultTag.Value.Name = 
            command.TagUpdateDTO.Name is null ? resultTag.Value.Name : command.TagUpdateDTO.Name;
        resultTag.Value.Description = 
            command.TagUpdateDTO.Description is null ? resultTag.Value.Description : command.TagUpdateDTO.Description;

        Result result = await _tagRepository.UpdateTagAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
