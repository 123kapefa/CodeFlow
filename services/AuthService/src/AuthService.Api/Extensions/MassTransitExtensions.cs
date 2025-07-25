using AuthService.Infrastructure;

using MassTransit;

namespace AuthService.Api.Extensions;

public static class MassTransitExtensions {

  public static WebApplicationBuilder AddAuthMessaging (this WebApplicationBuilder builder) {
    
    builder.Services.AddMassTransit (x => {
      x.SetKebabCaseEndpointNameFormatter ();

        x.AddEntityFrameworkOutbox<AuthServiceDbContext>(o => {
            o.UsePostgres();
            o.UseBusOutbox();
      });

      x.UsingRabbitMq ((ctx, cfg) => {

        cfg.Host ("rabbitmq", "/", h => {
          h.Username ("guest");
          h.Password ("guest");
        });

        cfg.ConfigureEndpoints (ctx);
      });
    });

    return builder;
  }

}