using ApiGateway.Application.Clients;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;


namespace ApiGateway.Api.Extensions;

public static class ServiceCollectionExtensions {

  public static WebApplicationBuilder AddApiClientsWithResilience (this WebApplicationBuilder builder) {
    builder.Services.AddHttpContextAccessor();
    
    builder.Services.AddHttpClient<UserApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:User"]!);
      })
     .AddStandardResilienceHandler(); 
    
    builder.Services.AddHttpClient<QuestionApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:Question"]!);
      })
     .AddStandardResilienceHandler(); 
    
    builder.Services.AddHttpClient<AnswerApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:Answer"]!);
      })
     .AddStandardResilienceHandler(); 
    
    builder.Services.AddHttpClient<CommentApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:Comment"]!);
      })
     .AddStandardResilienceHandler(); 
    
    builder.Services.AddHttpClient<TagApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:Tag"]!);
      })
     .AddStandardResilienceHandler(); 
    
    builder.Services.AddHttpClient<ReputationApi>(client =>
      {
        client.BaseAddress = new Uri(builder.Configuration["Services:Reputation"]!);
      })
     .AddStandardResilienceHandler(); 

    return builder;
  }
  
  public static IHttpClientBuilder AddStandardResilienceHandler(this IHttpClientBuilder builder)
  {
    // Пример двух политик: retry + circuit breaker
    var retryPolicy = HttpPolicyExtensions
     .HandleTransientHttpError() // HttpRequestException, 5XX и т.д.
     .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
          // Логирование, если надо
        }
      );

    var circuitBreakerPolicy = HttpPolicyExtensions
     .HandleTransientHttpError()
     .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 2,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, timespan) =>
        {
          // Логирование и т.д.
        },
        onReset: () =>
        {
          // Логирование и т.д.
        }
      );

    // Подключаем обе политики по цепочке
    return builder
     .AddPolicyHandler(retryPolicy)
     .AddPolicyHandler(circuitBreakerPolicy);
  }


}