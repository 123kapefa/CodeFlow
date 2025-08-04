using Messaging.Broker;

using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Extensions;

public static class MessagingExtensions
{
  public static IServiceCollection AddMessaging(this IServiceCollection services)
  {
    services.AddScoped<IMessageBroker, MassTransitMessageBroker>();
    return services;
  }
}