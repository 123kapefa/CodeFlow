using System;

using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using MassTransit;

using Microsoft.AspNetCore.Builder;

using ReputationService.Application.Consumers;
using ReputationService.Infrastructure;

namespace ReputationService.Api.Extensions;

public static class MassTransitExtensions {

  public static WebApplicationBuilder AddReputationMessaging( this WebApplicationBuilder builder ) {

    builder.Services.AddMassTransit(x => {
      x.SetKebabCaseEndpointNameFormatter();

      x.AddEntityFrameworkOutbox<ReputationServiceDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
      });

      x.AddConsumer<QuestionVotedConsumer> ();
      x.AddConsumer<AnswerVotedConsumer> ();
      x.AddConsumer<AcceptedAnswerChangedConsumer> ();
            
      x.UsingRabbitMq(( ctx, cfg ) => {
        cfg.Host("rabbitmq", "/", h => {
          h.Username("guest");
          h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("reputation-service.question-vote-changed", e =>
        {
          e.Bind<QuestionVoted>();

          e.ConfigureConsumer<QuestionVotedConsumer>(ctx);
          e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
          e.PrefetchCount = 16;
        });
        
        cfg.ReceiveEndpoint("reputation-service.answer-vote-changed", e =>
        {
          e.Bind<Contracts.Publishers.VoteService.AnswerVoted>();

          e.ConfigureConsumer<AnswerVotedConsumer>(ctx);
          e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
          e.PrefetchCount = 16;
        });

        cfg.ReceiveEndpoint("reputation-service.accepted-answer-changed", e =>
        {
          e.Bind<AnswerAccepted>();

          e.ConfigureConsumer<AcceptedAnswerChangedConsumer>(ctx);
          e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
      });
    });

    return builder;
  }

}