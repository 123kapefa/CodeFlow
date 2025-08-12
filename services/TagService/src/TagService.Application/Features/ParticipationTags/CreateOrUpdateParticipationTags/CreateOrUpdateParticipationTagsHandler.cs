using System.Globalization;

using Abstractions.Commands;

using Ardalis.Result;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.CreateOrUpdateParticipationTags;

public class CreateOrUpdateParticipationTagsHandler : ICommandHandler<CreateOrUpdateParticipationTagsCommand> {

  private readonly ITagRepository _tagRepository;
  private readonly IUserTagParticipationRepository _participationRepository;
    private readonly ILogger<CreateOrUpdateParticipationTagsHandler> _logger;

    public CreateOrUpdateParticipationTagsHandler( ITagRepository tagRepository, IUserTagParticipationRepository participationRepository, ILogger<CreateOrUpdateParticipationTagsHandler> logger ) {
        _tagRepository = tagRepository;
        _participationRepository = participationRepository;
        _logger = logger;
    }

    public async Task<Result> Handle (
    CreateOrUpdateParticipationTagsCommand command
    , CancellationToken cancellationToken) {
    DateTime now = DateTime.UtcNow;

    var tagResult = await _tagRepository.GetTagsByIdAsync (
      command.Tags, cancellationToken);

    List<int> allTagIds = tagResult.Value.Select (t => t.Id).ToList ();

    if (!tagResult.IsSuccess)
      return Result.Error (new ErrorList (tagResult.Errors));

    await using IDbContextTransaction tx = await _tagRepository.BeginTransactionAsync (cancellationToken);
    try {
      foreach (var tag in tagResult.Value) {
        if (tag.DailyCountUpdatedAt is null || (now.Date > tag.DailyCountUpdatedAt.Value.Date)) {
          tag.DailyCountUpdatedAt = now;
          tag.DailyRequestCount = 0;
        }

        int currWeek = ISOWeek.GetWeekOfYear (now);
        var lastUpdate = tag.WeeklyCountUpdatedAt;

        // проверка недели
        if (lastUpdate is null || ISOWeek.GetWeekOfYear (lastUpdate.Value) < currWeek ||
            lastUpdate.Value.Year < now.Year) {
          tag.WeeklyCountUpdatedAt = now;
          tag.WeeklyRequestCount = 0;
        }

        tag.DailyRequestCount += 1;
        tag.WeeklyRequestCount += 1;
        tag.CountQuestion += 1;
      }

      var participationMap =
        await _participationRepository.GetByUserAndTagIdsAsync (command.UserId, allTagIds, cancellationToken);

      var participationsToCreate = new List<UserTagParticipation> ();
      var questionsLinksToCreate = new List<UserTagParticipationQuestion> ();

      foreach (int tagId in allTagIds) {
        if (!participationMap.TryGetValue (tagId, out var p)) {
          p = UserTagParticipation.Create (command.UserId, tagId, isAnswer: false);
          participationsToCreate.Add (p);
          participationMap[tagId] = p;
        }
        else {
          p.QuestionsCreated += 1;
          p.LastActiveAt = now;
        }

        if (command.QuestionId is Guid qid) {
          UserTagParticipationQuestion link = UserTagParticipationQuestion.Create (qid);
          link.UserTagParticipationId = p.Id;
          questionsLinksToCreate.Add (link);
        }
      }

      if (participationsToCreate.Count > 0)
        await _participationRepository.AddRangeAsync (participationsToCreate, cancellationToken);

      // если у UserTagParticipationQuestion нет конструктора с установкой UTP Id, сделаем так:
      if (command.QuestionId is Guid questionId) {
        // Для новых UTP Id появятся только после сохранения — разделим на два шага.
        // 3.1. Сохраняем участия, чтобы получить их Id
        if (participationsToCreate.Count > 0)
          await _tagRepository.SaveChangesAsync (cancellationToken);

        List<UserTagParticipationQuestion> questionLinks = new List<UserTagParticipationQuestion> (allTagIds.Count);
        foreach (var tagId in allTagIds) {
          var p = participationMap[tagId];
          var link = UserTagParticipationQuestion.Create (questionId);
          link.UserTagParticipationId = p.Id;
          questionLinks.Add (link);
        }

        await _participationRepository.AddQuestionsRangeAsync (questionLinks, cancellationToken);
      }

      // Итоговый коммит
      await _tagRepository.SaveChangesAsync (cancellationToken);
      await tx.CommitAsync (cancellationToken);
      return Result.Success ();

    }
    catch (Exception) {
            _logger.LogError("\n\nFAILED TRANSACTION\n\n");
            Console.WriteLine("\n\nFAILED TRANSACTION\n\n");
      await tx.RollbackAsync (cancellationToken);
      return Result.Error ("Ошибка при обновлении тегов.");
    }
  }

}