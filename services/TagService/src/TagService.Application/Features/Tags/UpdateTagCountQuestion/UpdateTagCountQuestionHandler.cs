using Ardalis.Result;
using Contracts.Commands;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;

//TODO УДАЛИТЬ?? ИСПОЛЬЗОВАТЬ UpdateTagRequestCommand???
public class UpdateTagCountQuestionHandler : ICommandHandler<UpdateTagCountQuestionCommand> {

    private readonly ITagRepository _tagRepository;

    public UpdateTagCountQuestionHandler( ITagRepository tagRepository ) {
        _tagRepository = tagRepository;
    }


    public async Task<Result> Handle( UpdateTagCountQuestionCommand command, CancellationToken token ) {

        if(string.IsNullOrEmpty(command.Name))
            return Result.Error("Некорректный аргумент запроса");

        if(command.Count == 0)
            return Result.Error("Количество не может быть равно 0");

        Result<Tag> resultTag = await _tagRepository.GetTagByNameAsync(command.Name, token);
        if(!resultTag.IsSuccess)
            return Result.Error(new ErrorList(resultTag.Errors));

        resultTag.Value.CountQuestion += command.Count;

        Result result = await _tagRepository.UpdateTagAsync(resultTag.Value, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
