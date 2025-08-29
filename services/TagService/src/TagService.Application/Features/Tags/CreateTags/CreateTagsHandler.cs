using System.Text.Json;

using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.TagService;
using Contracts.Responses.TagService;

using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.CreateTags;

public class CreateTagsHandler : ICommandHandler<EnsureTagsResponse , CreateTagsCommand> {

  private readonly ITagRepository _tagRepository;
  
  public CreateTagsHandler (ITagRepository tagRepository) {
    _tagRepository = tagRepository;
  }

  public async Task<Result<EnsureTagsResponse >> Handle (CreateTagsCommand command, CancellationToken cancellationToken) {
    
    var tagNames = command.Request.Names;

    var existingResult = await _tagRepository.GetTagsByNamesAsync(tagNames, cancellationToken);
    if (!existingResult.IsSuccess)
      return Result<EnsureTagsResponse>.Error("Ошибка при получении существующих тегов по имени.");

    var existingTags = existingResult.Value;

    // Выделяем имена, которых ещё нет среди существующих тегов
    var existingNamesSet = existingTags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
    var newNames = tagNames
     .Where(n => !existingNamesSet.Contains(n))
     .Distinct(StringComparer.OrdinalIgnoreCase) // На всякий случай убираем дубли в новых именах
     .ToList();
    
    // Создаём новые сущности Tag для каждого нового имени
    var createdTags = new List<Tag>();
    if (newNames.Any())
    {
      foreach (var name in newNames)
      {
        var tag = Tag.Create(name, description: null);
        createdTags.Add(tag);
      }

      // Добавляем и сохраняем их в репо
      await _tagRepository.AddRangeAsync(createdTags, cancellationToken);
      await _tagRepository.SaveChangesAsync(cancellationToken);
    }
    
    // Формируем результат: все имена + их Id (для уже существующих и новых)
    // Предположим, в CreateTagsResponse вы возвращаете просто список строк или же список DTO.
    // Если нужно DTO, придётся маппить. Сейчас для простоты вернём список строк (существующие + новые).
    var allTags = existingTags
     .Select(t => new { t.Id, t.Name })
     .Concat(createdTags.Select(t => new { t.Id, t.Name }))
     .OrderBy(x => x.Name) // сортируем, если нужно
     .ToList();

    // Преобразуем в CreateTagsResponse (или как вы планируете возвращать данные)
    var response = new EnsureTagsResponse (
      allTags.Select(t => t.Id ).ToList()
    );

    return Result<EnsureTagsResponse >.Success(response);
  }

}