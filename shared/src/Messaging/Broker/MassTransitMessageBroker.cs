using MassTransit;

namespace Messaging.Broker;

public class MassTransitMessageBroker : IMessageBroker {

  private readonly IPublishEndpoint _publish;

  public MassTransitMessageBroker(IPublishEndpoint publish)
  {
    _publish = publish;
  }

  public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    where T : class
    => _publish.Publish(message, cancellationToken);

}