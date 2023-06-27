namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients;
  using JamesPChadwick.Ingestion.Messages;
  using Microsoft.Extensions.Logging;

  public interface IMessagingService
  {
    Task PublishMessages(Guid scope);

    void QueueMessageAsync(Infrastructure.Messaging.Messages.Message payload);
  }

  public class MessagingService : IMessagingService
  {
    private readonly IMessagingClient messagingClient;
    private readonly IMessageRepository messageRepository;
    private readonly ILogger<MessagingService> logger;
    private readonly List<Type> messageTypes = new ();

    public MessagingService(
      IMessagingClient messagingClient,
      IMessageRepository messageRepository,
      ILogger<MessagingService> logger)
    {
      this.messagingClient = messagingClient;
      this.messageRepository = messageRepository;
      this.logger = logger;

      messageTypes.Add(typeof(FileDiscovered));
    }

    public virtual async Task PublishMessages(Guid scope)
    {
      var unpublishedMessages = await messageRepository.FindByStatus(MessageStatus.Unpublished, scope);

      foreach (var unpublishedMessage in unpublishedMessages)
      {
        try
        {
          await UpdateMessageStatus(unpublishedMessage.Guid, MessageStatus.InProgress);

          var messageType = messageTypes.Single(type => type.Name == unpublishedMessage.Type);
          var messagePayload = JsonSerializer.Deserialize(unpublishedMessage.Payload, messageType) as Infrastructure.Messaging.Messages.Message;
          await messagingClient.Publish(messagePayload ?? throw new ArgumentNullException(nameof(messagePayload)));

          await UpdateMessageStatus(unpublishedMessage.Guid, MessageStatus.Published);
        }
        catch (Exception exception)
        {
          logger.LogError(exception, $"Publish failed for message ${unpublishedMessage.Guid}: ${exception.Message}");
          await UpdateMessageStatus(unpublishedMessage.Guid, MessageStatus.Failed);
        }
      }
    }

    public virtual void QueueMessageAsync(Infrastructure.Messaging.Messages.Message payload)
    {
      var message = new Domain.Aggregates.MessageAggregate.Message(
        payload.Id,
        payload.GetType().Name,
        JsonSerializer.Serialize(payload, payload.GetType()),
        payload.TimeStamp);

      message.SetScope(messageRepository.UnitOfWork.TransactionId ?? throw new NullReferenceException());

      messageRepository.Add(message);
    }

    private async Task UpdateMessageStatus(Guid guid, MessageStatus status)
    {
      var message = await messageRepository.FindByGuid(guid);

      message.SetStatus(status);

      messageRepository.Update(message);

      await messageRepository.UnitOfWork.SaveEntitiesAsync();
    }
  }
}
