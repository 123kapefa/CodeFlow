using MassTransit;
using UserService.Application.Consumers;
using UserService.Infrastructure.Data;

namespace UserService.Api.Extensions;

public static class MassTransitExtensions {
    
    public static WebApplicationBuilder AddUserMessaging (this WebApplicationBuilder builder) {
    
        builder.Services.AddMassTransit (x =>
        {
            x.SetKebabCaseEndpointNameFormatter ();

            x.AddEntityFrameworkOutbox<UserServiceDbContext>(o => {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<UserRegisteredConsumer> ();
            x.AddConsumer<UserLoggedConsumer>();
            x.AddConsumer<QuestionVotedConsumer>();

            x.UsingRabbitMq ((ctx, cfg) =>
            {
                cfg.Host ("rabbitmq", "/", h =>
                {
                h.Username ("guest");
                    h.Password ("guest");
                });        
                               

                cfg.ReceiveEndpoint("user-service.user-registered", e => {
                    
                    e.ConfigureConsumer<UserRegisteredConsumer>(ctx);                    
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));                    
                });

                cfg.ReceiveEndpoint("user-service.user-logged", e => {

                    e.ConfigureConsumer<UserLoggedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ReceiveEndpoint("user-service.question-voted", e => {

                    e.ConfigureConsumer<QuestionVotedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

            });
        });
        
        return builder;
    }


}