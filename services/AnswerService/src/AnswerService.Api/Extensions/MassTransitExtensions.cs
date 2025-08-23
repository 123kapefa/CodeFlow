using AnswerService.Application.Consumers;
using AnswerService.Infrastructure;

using Contracts.Publishers.AnswerService;

using MassTransit;

namespace AnswerService.Api.Extensions;

public static class MassTransitExtensions {

    public static WebApplicationBuilder AddAnswerMessaging( this WebApplicationBuilder builder ) {

        builder.Services.AddMassTransit(x => {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<AnswerServiceDbContext>(o => {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<AnswerVotedConsumer> ();
            x.AddConsumer<AnswerAcceptedConsumer> ();
            x.AddConsumer<UserDeletedConsumer> ();
            
            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("answer-service.answer-voted", e => {
                    e.Bind<AnswerVotedConsumer>();

                    e.ConfigureConsumer<AnswerVotedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    e.PrefetchCount = 16;
                });
                
                cfg.ReceiveEndpoint("answer-service.answer-accepted", e => {

                    e.ConfigureConsumer<AnswerAcceptedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ReceiveEndpoint("answer-service.user-deleted", e => {
                
                    e.ConfigureConsumer<UserDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
        });

        return builder;
    }

}