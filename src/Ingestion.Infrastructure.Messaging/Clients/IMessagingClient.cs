namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients
{
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;

  public interface IMessagingClient
  {
    Task Publish(Message message);

    void Subscribe<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>;

    void Unsubscribe<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>;
  }
}
