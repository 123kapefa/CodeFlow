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

            x.AddConsumer<AnswerAcceptedConsumer> ();
            
            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                // cfg.ReceiveEndpoint("answer-service.answer-accepted", e => {
                //
                //     e.ConfigureConsumer<AnswerAcceptedConsumer>(ctx);
                //     e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                // });
                
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return builder;
    }

}