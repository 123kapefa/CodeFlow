using AnswerService.Infrastructure;
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

            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return builder;
    }

}