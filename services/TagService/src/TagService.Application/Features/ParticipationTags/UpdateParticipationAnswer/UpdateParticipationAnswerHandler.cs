using Abstractions.Commands;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore.Storage;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.UpdateParticipationAnswer;

public class UpdateParticipationAnswerHandler : ICommandHandler<UpdateParticipationAnswerCommand> {

    private readonly IUserTagParticipationRepository _repository;

    public UpdateParticipationAnswerHandler( IUserTagParticipationRepository repository ) {
        _repository = repository;
    }

    public async Task<Result> Handle( UpdateParticipationAnswerCommand command, CancellationToken token ) {

        DateTime now = DateTime.UtcNow;

        List<int> tagIds = command.TagDTOs.Select(t => t.TagId).ToList();
        Dictionary<int, UserTagParticipation> participationMap = await _repository
                .GetByUserAndTagIdsAsync(command.UserId, tagIds, token);

        List<UserTagParticipation> participationsToCreate = new List<UserTagParticipation>();
        List<UserTagParticipationQuestion> links = new List<UserTagParticipationQuestion>(tagIds.Count);

        await using IDbContextTransaction tx = await _repository.BeginTransactionAsync(token);
        try {
            // создаём/обновляем участия
            foreach(int tagId in tagIds) {
                if(!participationMap.TryGetValue(tagId, out var p)) {
                    p = UserTagParticipation.Create(command.UserId, tagId, isAnswer: true);                    
                    p.LastActiveAt = now;

                    participationsToCreate.Add(p);
                    participationMap[tagId] = p;
                }
                else {
                    p.AnswersWritten += 1;
                    p.LastActiveAt = now;
                }
            }

            // сохраняем участия
            if(participationsToCreate.Count > 0)
                await _repository.AddRangeAsync(participationsToCreate, token);

            await _repository.SaveChangesAsync(token);

            // формируем ссылки вопрос-участие
            foreach(var tagId in tagIds) {
                UserTagParticipation p = participationMap[tagId];

                UserTagParticipationQuestion link = UserTagParticipationQuestion.Create(command.QuestionId);
                link.UserTagParticipationId = p.Id;

                links.Add(link);
            }

            if(links.Count > 0)
                await _repository.AddQuestionsRangeAsync(links, token);

           
            await _repository.SaveChangesAsync(token);
            await tx.CommitAsync(token);

            return Result.Success();
        }
        catch {
            await tx.RollbackAsync(token);
            return Result.Error("Ошибка при обновлении участия пользователя по тэгам.");
        }
    }
}
