using Abstractions.Commands;

using Ardalis.Result;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

using Contracts.DTOs.TagService;

using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;

//TODO ПРИ СОЗДАНИИ ВОПРОСА
public class UpdateTagCountQuestionHandler : ICommandHandler<UpdateTagCountQuestionCommand> {

    private readonly ITagRepository _tagRepository;
    private readonly IUserTagParticipationRepository _participationRepository;

    public UpdateTagCountQuestionHandler( ITagRepository tagRepository, IUserTagParticipationRepository participationRepository ) {
        _tagRepository = tagRepository;
        _participationRepository = participationRepository;
    }

    public async Task<Result> Handle( UpdateTagCountQuestionCommand command, CancellationToken token ) {

        List<int> tagIds = command.TagDTOs.Where(t => t.TagId.HasValue).Select(t => t.TagId.Value).ToList();
        List<QuestionTagDTO> newTags = command.TagDTOs.AsEnumerable().Where(t => !t.TagId.HasValue).ToList();
        DateTime now = DateTime.UtcNow;

        Result<List<Tag>> tagResult = await _tagRepository.GetTagsByIdAsync(tagIds, token);
        if(!tagResult.IsSuccess)
            return Result.Error(new ErrorList(tagResult.Errors));

        List<Tag> existingTags  = tagResult.Value;

        await using IDbContextTransaction tx = await _tagRepository.BeginTransactionAsync(token);
        try {
            foreach(Tag tag in existingTags ) {

                if(tag.DailyCountUpdatedAt is null || (now.Date > tag.DailyCountUpdatedAt.Value.Date)) {
                    tag.DailyCountUpdatedAt = now;
                    tag.DailyRequestCount = 0;
                }

                int currWeek = ISOWeek.GetWeekOfYear(now);
                var lastUpdate = tag.WeeklyCountUpdatedAt;

                // проверка недели
                if(lastUpdate is null || ISOWeek.GetWeekOfYear(lastUpdate.Value) < currWeek || lastUpdate.Value.Year < now.Year) {
                    tag.WeeklyCountUpdatedAt = now;
                    tag.WeeklyRequestCount = 0;
                }

                tag.DailyRequestCount += 1;
                tag.WeeklyRequestCount += 1;
                tag.CountQuestion += 1;
            }

            List<Tag> createdTags  = [];

            foreach(var newTag in newTags) {
                var tag = Tag.Create(newTag.Name, description: null);
                tag.CountQuestion = 1;
                tag.DailyRequestCount = 1;
                tag.WeeklyRequestCount = 1;
                tag.DailyCountUpdatedAt = now;
                tag.WeeklyCountUpdatedAt = now;

                createdTags .Add(tag);
            }

            await _tagRepository.AddRangeAsync(createdTags , token);
            // Сохраняем, чтобы получить Id у созданных тегов
            await _tagRepository.SaveChangesAsync(token);

            List<int> allTagIds = existingTags.Select(t => t.Id)
                                       .Concat(createdTags.Select(t => t.Id))
                                       .ToList();

            // Подтягиваем уже существующие участия пользователя
            var participationMap = await _participationRepository
                .GetByUserAndTagIdsAsync(command.UserId, allTagIds, token);

            var participationsToCreate = new List<UserTagParticipation>();
            var questionsLinksToCreate = new List<UserTagParticipationQuestion>();

            foreach(int tagId in allTagIds) {
                if(!participationMap.TryGetValue(tagId, out var p)) {
                    p = UserTagParticipation.Create(command.UserId, tagId, isAnswer: false);
                    participationsToCreate.Add(p);
                    participationMap[tagId] = p;
                }
                else {
                    p.QuestionsCreated += 1;
                    p.LastActiveAt = now;
                }

                if(command.QuestionId is Guid qid) {
                    UserTagParticipationQuestion link = UserTagParticipationQuestion.Create(qid);
                    link.UserTagParticipationId = p.Id;   
                    questionsLinksToCreate.Add(link);
                }
            }

            if(participationsToCreate.Count > 0)
                await _participationRepository.AddRangeAsync(participationsToCreate, token);

            // если у UserTagParticipationQuestion нет конструктора с установкой UTP Id, сделаем так:
            if(command.QuestionId is Guid questionId) {
                // Для новых UTP Id появятся только после сохранения — разделим на два шага.
                // 3.1. Сохраняем участия, чтобы получить их Id
                if(participationsToCreate.Count > 0)
                    await _tagRepository.SaveChangesAsync(token);

                List<UserTagParticipationQuestion> questionLinks = new List<UserTagParticipationQuestion>(allTagIds.Count);
                foreach(var tagId in allTagIds) {
                    var p = participationMap[tagId];
                    var link = UserTagParticipationQuestion.Create(questionId);
                    link.UserTagParticipationId = p.Id;
                    questionLinks.Add(link);
                }

                await _participationRepository.AddQuestionsRangeAsync(questionLinks, token);
            }

            // Итоговый коммит
            await _tagRepository.SaveChangesAsync(token);
            await tx.CommitAsync(token);

            return Result.Success();
        }
        catch {
            await tx.RollbackAsync(token);
            return Result.Error("Ошибка при обновлении тэгов");
        }

    }

}
