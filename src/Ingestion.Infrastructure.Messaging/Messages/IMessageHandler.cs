namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages
{
  using System.Threading.Tasks;

  public interface IMessageHandler
  {
  }

  public interface IMessageHandler<in TMessage> : IMessageHandler
    where TMessage : Message
  {
    Task Handle(TMessage message);
  }
}
