using System.Text.Json;

using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.TagService;
using Contracts.Responses.TagService;

using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.CreateTags;

public class CreateTagsHandler : ICommandHandler<CreateTagsResponse, CreateTagsCommand> {

  private readonly ITagRepository _tagRepository;
  
  public CreateTagsHandler (ITagRepository tagRepository) {
    _tagRepository = tagRepository;
  }

  public async Task<Result<CreateTagsResponse>> Handle (CreateTagsCommand command, CancellationToken cancellationToken) {
    
    // Шаг 1: Разделяем входной список на существующие и новые теги
    var existingTagIds = command.QuestionTags
     .Where(t => t.Id.HasValue)
     .Select(t => t.Id!.Value)
     .ToList();

    var newTags = command.QuestionTags
     .Where(t => !t.Id.HasValue)
     .Select(t => new CreateTagDto { Name = t.Name, Description = null })
     .ToList();

    foreach (var createTagDto in  newTags) {
      var findTag = await _tagRepository.GetTagByNameAsync (createTagDto.Name, cancellationToken);

      if (findTag.IsSuccess) {
        existingTagIds.Add(findTag.Value.Id);
        newTags.Remove(createTagDto);
      }
    }
    
    var existingTagsResult = await _tagRepository.GetTagsByIdAsync(existingTagIds, cancellationToken);
    if (!existingTagsResult.IsSuccess) {
      return Result<CreateTagsResponse>.Error("Ошибка при получении существующих тегов.");
    }

    var existingTags = existingTagsResult.Value;
    
    var createdTags = new List<Tag>();
    
    if (newTags.Any()) {
      foreach (var tagDto in newTags) {
        var tag = Tag.Create(tagDto.Name, tagDto.Description);
        createdTags.Add(tag);
      }

      await _tagRepository.AddRangeAsync(createdTags, cancellationToken);
      await _tagRepository.SaveChangesAsync(cancellationToken);
    }
    
    var allTags = existingTags
     .Select(t => new CreateTagDto { Id = t.Id, Name = t.Name })
     .Concat(createdTags.Select(t => new CreateTagDto { Id = t.Id, Name = t.Name }))
     .ToList();

    return Result<CreateTagsResponse>.Success(new CreateTagsResponse(allTags));
  }

}