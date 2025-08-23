using MassTransit;

using QuestionService.Application.Consumers;
using QuestionService.Infrastructure.Data;

namespace QuestionService.Api.Extensions;

public static class MassTransitExtensions {

    public static WebApplicationBuilder AddQuestionMessaging( this WebApplicationBuilder builder ) {

        builder.Services.AddMassTransit(x => {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<QuestionServiceDbContext>(o => {
                o.UsePostgres();
                o.UseBusOutbox();
            });
            
            x.AddConsumer<QuestionVotedConsumer> ();
            x.AddConsumer<AnswerCreatedConsumer> ();
            x.AddConsumer<AnswerDeletedConsumer> ();
            

            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("question-service.question-voted", e => {
                    e.Bind<Contracts.Publishers.VoteService.QuestionVoted>();
                
                    e.ConfigureConsumer<QuestionVotedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    e.PrefetchCount = 16;
                });
                
                cfg.ReceiveEndpoint("question-service.answer-created", e => {

                    e.ConfigureConsumer<AnswerCreatedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ReceiveEndpoint("question-service.answer-deleted", e => {

                    e.ConfigureConsumer<AnswerDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return builder;
    }
}
