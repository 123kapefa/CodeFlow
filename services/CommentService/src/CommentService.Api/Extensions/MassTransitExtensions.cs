using CommentService.Application.Consumers;
using CommentService.Infrastructure.Data;

using MassTransit;

namespace CommentService.Api.Extensions;

public static class MassTransitExtensions {

    public static WebApplicationBuilder AddCommentMessaging( this WebApplicationBuilder builder ) {

        builder.Services.AddMassTransit(x => {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<CommentServiceDbContext>(o => {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<AnswerDeletedConsumer> ();
            x.AddConsumer<UserDeletedConsumer> ();

            x.UsingRabbitMq(( ctx, cfg ) => {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ReceiveEndpoint("comment-service.answer-deleted", e => {

                    e.ConfigureConsumer<AnswerDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
                
                cfg.ReceiveEndpoint("comment-service.user-deleted", e => {

                    e.ConfigureConsumer<UserDeletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
        });

        return builder;
    }
}
