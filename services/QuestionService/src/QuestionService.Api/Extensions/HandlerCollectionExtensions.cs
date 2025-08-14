using Abstractions.Commands;
using Ardalis.Result;
using Contracts.DTOs.QuestionService;
using Contracts.Responses.QuestionService;
using FluentValidation;
using QuestionService.Application.Features.CreateQuestion;
using QuestionService.Application.Features.DeleteQuestion;
using QuestionService.Application.Features.GetQuestion;
using QuestionService.Application.Features.GetQuestionHistory;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Application.Features.GetQuestionsByIds;
using QuestionService.Application.Features.GetQuestionsByTags;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Application.Features.GetQuestionTags;
using QuestionService.Application.Features.GetUserQuestions;
using QuestionService.Application.Features.ReduceQuestionAnswers;
using QuestionService.Application.Features.UpdateQuestion;
using QuestionService.Application.Features.UpdateQuestionAccept;
using QuestionService.Application.Features.UpdateQuestionAnswers;
using QuestionService.Application.Features.UpdateQuestionView;
using QuestionService.Application.Features.UpdateQuestionVote;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Repositories;

namespace QuestionService.Api.Extensions;

public static class HandlerCollectionExtensions {

    public static WebApplicationBuilder AddHandlers( this WebApplicationBuilder builder ) {

        builder.Services.AddScoped<IQuestionServiceRepository, QuestionServiceRepository>();
        builder.Services.AddScoped<IValidator<CreateQuestionCommand>, CreateQuestionValidator>();
        builder.Services.AddScoped<IValidator<UpdateQuestionCommand>, UpdateQuestionValidator>();

        builder.Services.AddScoped<ICommandHandler<QuestionDTO, GetQuestionCommand>, GetQuestionHandler>();
        builder.Services.AddScoped<ICommandHandler<QuestionShortDTO, GetQuestionShortCommand>, GetQuestionShortHandler>();
        builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand>, GetQuestionHistoryHandler>();
        builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionTagDTO>, GetQuestionTagsCommand>, GetQuestionTagsHandler>();
        builder.Services.AddScoped<ICommandHandler<CreatedQuestionResponse, CreateQuestionCommand>, CreateQuestionHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateQuestionCommand>, UpdateQuestionHandler>();
        builder.Services.AddScoped<ICommandHandler<DeleteQuestionCommand>, DeleteQuestionHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateQuestionAcceptCommand>, UpdateQuestionAcceptHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateQuestionViewCommand>, UpdateQuestionViewHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateQuestionVoteCommand>, UpdateQuestionVoteHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateQuestionAnswersCommand>, UpdateQuestionAnswersHandler>();
        builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsCommand>, GetQuestionsHandler>();
        builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsByIdsCommand>, GetQuestionsByIdsHandler>();
        builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetUserQuestionsCommand>, GetUserQuestionsHandler>();
        builder.Services.AddScoped<ICommandHandler<ReduceQuestionAnswersCommand>, ReduceQuestionAnswersHandler>();
        builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionShortDTO>, GetQuestionsByTagsCommand>, GetQuestionsByTagsHandler>();

        return builder;
    }

}