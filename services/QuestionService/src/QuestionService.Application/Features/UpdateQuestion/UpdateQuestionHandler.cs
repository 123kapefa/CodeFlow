using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Contracts.Commands;
using FluentValidation;
using QuestionService.Application.Features.CreateQuestion;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.UpdateQuestion;

public class UpdateQuestionHandler : ICommandHandler<UpdateQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;
    private readonly IValidator<UpdateQuestionCommand> _validator;

    public UpdateQuestionHandler( IQuestionServiceRepository questionServiceRepository, IValidator<UpdateQuestionCommand> validator ) {
        _questionServiceRepository = questionServiceRepository;
        _validator = validator;
    }


    // TODO ПОДУМАТЬ НАД ТЕМ КАК СДЕЛАТЬ ВСЕ ЭТО В ОДИН SaveChange
    public async Task<Result> Handle( UpdateQuestionCommand command, CancellationToken token ) {
        // 1) валидация
        var validateResult = await _validator.ValidateAsync(command, token);
        if(!validateResult.IsValid)
            return Result.Invalid(validateResult.AsErrors());


        // 2) загружаем вопрос
        Result<Question> question = await _questionServiceRepository.GetQuestionAsync(command.UpdateQuestionDTO.Id, token);

        if(!question.IsSuccess || question.Value is null)
            return Result.Error(
                new ErrorList(question.Errors.Any() ? question.Errors : ["Вопрос не найден"]));     
        
        question.Value.Content = command.UpdateQuestionDTO.Content;
        question.Value.UpdatedAt = DateTime.UtcNow;
        question.Value.UserEditorId = command.UpdateQuestionDTO.UserEditorId;

        Result questionResult = await _questionServiceRepository.UpdateQuestionAsync(question.Value, token);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        QuestionChangingHistory questionHistory = new QuestionChangingHistory {
            UserId = command.UpdateQuestionDTO.UserEditorId,
            Content = command.UpdateQuestionDTO.Content,
            UpdatedAt = DateTime.UtcNow,
            QuestionId = command.UpdateQuestionDTO.Id
        };

        Result historyResult = await _questionServiceRepository.CreateQuestionChangingHistoryAsync(questionHistory, token);

        if(!historyResult.IsSuccess)
            return Result.Error(new ErrorList(historyResult.Errors));

        List<QuestionTag> newTags = command.UpdateQuestionDTO.QuestionTagsDTO.Select(qt => new QuestionTag {
            TagId = qt.TagId,
            WatchedAt = DateTime.UtcNow,
            QuestionId = command.UpdateQuestionDTO.Id
        }).ToList();

        Result tagResult = await _questionServiceRepository.UpdateQuestionTagsAsync(command.UpdateQuestionDTO.Id, newTags, token);

        if(!tagResult.IsSuccess)
            return Result.Error(new ErrorList(tagResult.Errors));

        return Result.Success();

    }



    //public async Task<Result> Handle( UpdateQuestionCommand cmd, CancellationToken ct ) {
    //    // 1) валидация
    //    var vr = await _validator.ValidateAsync(cmd, ct);
    //    if(!vr.IsValid)
    //        return Result.Invalid(vr.AsErrors());

    //    // 2) загружаем вопрос
    //    var get = await _questionServiceRepository.GetQuestionAsync(cmd.UpdateQuestionDTO.Id, ct);
    //    if(!get.IsSuccess || get.Value is null)
    //        return Result.Error(new ErrorList(get.Errors.Any()
    //                                          ? get.Errors
    //                                          : ["Вопрос не найден"]));

    //    var q = get.Value;

    //    // 3) правим поля
    //    q.Title = cmd.UpdateQuestionDTO.Title;
    //    q.Content = cmd.UpdateQuestionDTO.Content;
    //    q.UpdatedAt = DateTime.UtcNow;
    //    q.UserEditorId = cmd.UpdateQuestionDTO.UserEditorId;

    //    // 4) синхронизируем теги
    //    var incoming = cmd.UpdateQuestionDTO.QuestionTagsDTO.Select(t => t.TagId).ToHashSet();
    //    var toRemove = q.QuestionTags.Where(t => !incoming.Contains(t.TagId)).ToList();
    //    _questionServiceRepository.RemoveQuestionTags(toRemove);      // ← только метим на удаление

    //    var existing = q.QuestionTags.Select(t => t.TagId).ToHashSet();
    //    foreach(var dto in cmd.UpdateQuestionDTO.QuestionTagsDTO.Where(t => !existing.Contains(t.TagId)))
    //        q.QuestionTags.Add(new QuestionTag { TagId = dto.TagId, WatchedAt = DateTime.UtcNow });

    //    // 5) пишем историю
    //    q.QuestionChangingHistories.Add(new QuestionChangingHistory {
    //        UserId = cmd.UpdateQuestionDTO.UserEditorId,
    //        Content = cmd.UpdateQuestionDTO.Content,
    //        UpdatedAt = DateTime.UtcNow
    //    });

    //    // 6) одно сохранение + обработка ошибок

    //    var r = await _questionServiceRepository.UpdateQuestionAsync(q,ct);          // ← единственный SaveChanges
    //    return r.IsSuccess ? Result.Success() : Result.Error(new ErrorList(r.Errors));

    //}



    //public async Task<Result> Handle( UpdateQuestionCommand cmd, CancellationToken ct ) {
    //    /* 1) валидация ------------------------------------------------------- */
    //    var vr = await _validator.ValidateAsync(cmd, ct);
    //    if(!vr.IsValid)
    //        return Result.Invalid(vr.AsErrors());

    //    /* 2) загружаем вопрос + коллекции ------------------------------------ */
    //    var get = await _questionServiceRepository.GetQuestionAsync(cmd.UpdateQuestionDTO.Id, ct);
    //    if(!get.IsSuccess || get.Value is null)
    //        return Result.Error(new ErrorList(get.Errors.Any()
    //                                          ? get.Errors
    //                                          : ["Вопрос не найден"]));

    //    var q = get.Value;                       // EF уже отслеживает объект

    //    /* 3) правим основные поля ------------------------------------------- */
    //    q.Title = cmd.UpdateQuestionDTO.Title;
    //    q.Content = cmd.UpdateQuestionDTO.Content;
    //    q.UpdatedAt = DateTime.UtcNow;
    //    q.UserEditorId = cmd.UpdateQuestionDTO.UserEditorId;

    //    /* 4) синхронизируем теги -------------------------------------------- */
    //    var incomingIds = cmd.UpdateQuestionDTO.QuestionTagsDTO
    //                          .Select(t => t.TagId)
    //                          .ToHashSet();

    //    // удалить те, которых больше нет в запросе
    //    var toRemove = q.QuestionTags
    //                    .Where(t => !incomingIds.Contains(t.TagId))
    //                    .ToList();
    //    _questionServiceRepository.RemoveQuestionTags(toRemove);      // просто помечаем на удаление

    //    // добавить новые
    //    var existingIds = q.QuestionTags.Select(t => t.TagId).ToHashSet();
    //    foreach(var dto in cmd.UpdateQuestionDTO.QuestionTagsDTO
    //                                .Where(t => !existingIds.Contains(t.TagId))) {
    //        q.QuestionTags.Add(new QuestionTag {
    //            TagId = dto.TagId,
    //            WatchedAt = DateTime.UtcNow      // серверная дата
    //        });
    //    }

    //    /* 5) записываем историю правок -------------------------------------- */
    //    q.QuestionChangingHistories.Add(new QuestionChangingHistory {
    //        UserId = cmd.UpdateQuestionDTO.UserEditorId,
    //        Content = cmd.UpdateQuestionDTO.Content,
    //        UpdatedAt = DateTime.UtcNow
    //    });

    //    /* 6) одно сохранение + обработка ошибок ----------------------------- */

    //    var r = await _questionServiceRepository.SaveAsync(ct);           // ← единственный SaveChanges
    //    return r.IsSuccess ? Result.Success() : Result.Error(new ErrorList(r.Errors));

    //}

}
