using MassTransit;

using UserService.Application.Consumers;
using UserService.Infrastructure.Data;

namespace UserService.Api.Extensions;

public static class MassTransitExtensions {

  public static WebApplicationBuilder AddUserMessaging (this WebApplicationBuilder builder) {
    
    builder.Services.AddMassTransit (x =>
    {
      x.SetKebabCaseEndpointNameFormatter ();

      x.AddEntityFrameworkOutbox<UserServiceDbContext> (o =>
      {
        o.UsePostgres ();
        o.UseBusOutbox ();
      });

      x.AddConsumer<UserRegisteredConsumer> ();

      x.UsingRabbitMq ((ctx, cfg) =>
      {
        cfg.Host ("rabbitmq", "/", h =>
        {
          h.Username ("guest");
          h.Password ("guest");
        });

        cfg.ReceiveEndpoint ("user-created", e => { e.ConfigureConsumer<UserRegisteredConsumer> (ctx); });

        cfg.ConfigureEndpoints (ctx);
      });
    });

    return builder;
  }

}