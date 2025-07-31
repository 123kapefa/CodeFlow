using MassTransit;

using TagService.Application.Consumers;
using TagService.Infrastructure.Data;

namespace TagService.Api.Extensions;

public static class MassTransitExtensions {

    public static WebApplicationBuilder AddTagMessaging( this WebApplicationBuilder builder ) {

        builder.Services.AddMassTransit(x => {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<TagServiceDbContext>(o => {
                o.UsePostgres();
                o.UseBusOutbox();
            });
            
            x.AddConsumer<AnswerCreatedConsumer> ();
            x.AddConsumer<AnswerDeletedConsumer> ();
            x.AddConsumer<UserDeletedConsumer> ();

            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("tag-service.answer-created", e => {

                    e.ConfigureConsumer<AnswerCreatedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ReceiveEndpoint("tag-service.answer-deleted", e => {

                    e.ConfigureConsumer<AnswerDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ReceiveEndpoint("tag-service.user-deleted", e => {

                    e.ConfigureConsumer<UserDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
        });

        return builder;
    }
}
