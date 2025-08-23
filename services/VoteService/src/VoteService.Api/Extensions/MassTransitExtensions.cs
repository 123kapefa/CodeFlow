using MassTransit;

using VoteService.Infrastructure;

namespace VoteService.Api.Extensions;

public static class MassTransitExtensions {

  public static WebApplicationBuilder AddVoteMessaging( this WebApplicationBuilder builder ) {

    builder.Services.AddMassTransit(x => {
      x.SetKebabCaseEndpointNameFormatter();

      x.AddEntityFrameworkOutbox<VoteServiceDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
      });
            
      x.UsingRabbitMq(( ctx, cfg ) => {
        cfg.Host("rabbitmq", "/", h => {
          h.Username("guest");
          h.Password("guest");
        });
      });
    });

    return builder;
  }

}