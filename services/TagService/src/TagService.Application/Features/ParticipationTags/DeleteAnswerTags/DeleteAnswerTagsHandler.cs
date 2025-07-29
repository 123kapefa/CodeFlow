using Abstractions.Commands;
using Ardalis.Result;
using System.IO;
using TagService.Domain.Entities;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.ParticipationTags.DeleteAnswerTags;

public class DeleteAnswerTagsHandler : ICommandHandler<DeleteAnswerTagsCommand> {

    private readonly IUserTagParticipationRepository _participationRepository;

    public DeleteAnswerTagsHandler( IUserTagParticipationRepository participationRepository ) {
        _participationRepository = participationRepository;
    }

    public async Task<Result> Handle( DeleteAnswerTagsCommand command, CancellationToken token ) {

        if(command.UserId == Guid.Empty || command.QuestionId == Guid.Empty || command.TagIds.Count() <= 0)
            return Result.Error("Пользователь, вопрос или теги не могут быть пустыми");


        IEnumerable<UserTagParticipation> parts = await _participationRepository
            .GetUserTagsAsync(command.UserId, command.TagIds, token);

        // список Id для удаления ссылок
        List<Guid> partIds = parts.Select(p => p.Id).ToList();        

        await using var tx = await _participationRepository.BeginTransactionAsync(token);
        try {

            Result delLinks = await _participationRepository
           .DeleteUserParticipationTags(command.QuestionId, partIds, token);

            if(!delLinks.IsSuccess) {
                await tx.RollbackAsync(token);
                return Result.Error(new ErrorList(delLinks.Errors));
            }

            foreach(var part in parts) {
                if(part.AnswersWritten > 0) 
                    part.AnswersWritten -= 1;   

                if(part.QuestionsCreated == 0 && part.AnswersWritten == 0)
                    _participationRepository.DeleteTagParticipation(part); 
            }

           
            await _participationRepository.SaveChangesAsync(token);
            await tx.CommitAsync(token);

            return Result.Success();
        }
        catch(Exception ex) {
            await tx.RollbackAsync(token);            
            return Result.Error("Ошибка при откате тегов ответа");
        }
    }
}
