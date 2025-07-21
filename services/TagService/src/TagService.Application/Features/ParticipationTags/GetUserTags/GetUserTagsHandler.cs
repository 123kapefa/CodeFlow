using Ardalis.Result;
using Contracts.Commands;
using TagService.Application.DTO;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.GetUserTags;

public class GetUserTagsHandler : ICommandHandler<PagedResult<IEnumerable<ParticipationDTO>>, GetUserTagsCommand> {

    private readonly IUserTagParticipationRepository _repository;

    public GetUserTagsHandler( IUserTagParticipationRepository repository ) {
        _repository = repository;
    }


    public async Task<Result<PagedResult<IEnumerable<ParticipationDTO>>>> Handle( 
        GetUserTagsCommand command, CancellationToken token ) {

        var resultTags = await _repository.GetTagsAsync( command.UserId, command.PageParams, command.SortParams, token);
        if(!resultTags.IsSuccess)
            return Result<PagedResult<IEnumerable<ParticipationDTO>>>.Error(new ErrorList(resultTags.Errors));

        //IEnumerable<ParticipationDTO> tags = resultTags.Value.items.Select(x => new ParticipationDTO { 
        //    UserId = x.Id,
        //    TagId = x.TagId,
        //    LastActiveAt = x.LastActiveAt,
        //    QuestionsCreated = x.QuestionsCreated,
        //    AnswersWritten = x.AnswersWritten,
        //    TagName = x.Tag.Name
        //});

        IEnumerable<ParticipationDTO> tags = resultTags.Value.items.Select(x => ParticipationDTO.Create(
            x.Id, x.TagId, x.LastActiveAt, x.QuestionsCreated, x.AnswersWritten, x.Tag.Name
            ));

        return Result<PagedResult<IEnumerable<ParticipationDTO>>>
            .Success(new PagedResult<IEnumerable<ParticipationDTO>>(resultTags.Value.pageInfo, tags));
    }

}
