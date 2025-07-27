using MassTransit;
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
