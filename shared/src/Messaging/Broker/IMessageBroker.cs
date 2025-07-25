namespace Messaging.Broker;

public interface IMessageBroker {

  Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    where T : class;

}