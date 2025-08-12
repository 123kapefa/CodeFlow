using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.TagService;

using TagService.Application.Extensions;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.GetTagsByIds;

public class GetTagsByIdsHandler : ICommandHandler<IEnumerable<TagDTO>, GetTagsByIdsCommand> {

  private readonly ITagRepository _tagRepository;
  
  public GetTagsByIdsHandler (ITagRepository tagRepository) {
    _tagRepository = tagRepository;
  }

  public async Task<Result<IEnumerable<TagDTO>>> Handle (GetTagsByIdsCommand command, CancellationToken cancellationToken) {
    var tagsResult = await _tagRepository.GetTagsAsync(command.TagIds, cancellationToken);
    if(!tagsResult.IsSuccess)
      return Result<IEnumerable<TagDTO>>.Error(new ErrorList(tagsResult.Errors));            

    IEnumerable<TagDTO> tags = tagsResult.Value.ToTagsDto();

    return Result<IEnumerable<TagDTO>>
     .Success(tags);
  }

}