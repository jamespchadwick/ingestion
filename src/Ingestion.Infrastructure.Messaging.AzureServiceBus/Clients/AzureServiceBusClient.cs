namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients.AzureServiceBus
{
  using System;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Autofac;
  using Azure.Messaging.ServiceBus;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Subscriptions;
  using Microsoft.Extensions.Logging;
  using Serilog.Context;

  public class AzureServiceBusClient : IMessagingClient
  {
    private readonly string topic;
    private readonly string subscription;
    private readonly AzureServiceBusClientConnection azureServiceBusClientConnection;
    private readonly SubscriptionManager subscriptionManager;
    private readonly ILifetimeScope lifetimeScope;
    private readonly ILogger<AzureServiceBusClient> logger;
    private readonly ServiceBusSender sender;
    private readonly ServiceBusProcessor processor;

    public AzureServiceBusClient(
      string topic,
      AzureServiceBusClientConnection azureServiceBusClientConnection,
      ILogger<AzureServiceBusClient> logger)
    {
      this.topic = topic;
      this.azureServiceBusClientConnection = azureServiceBusClientConnection;
      this.logger = logger;

      this.sender = azureServiceBusClientConnection.TopicClient.CreateSender(topic);
    }

    public AzureServiceBusClient(
      string topic,
      string subscription,
      AzureServiceBusClientConnection azureServiceBusClientConnection,
      SubscriptionManager subscriptionManager,
      ILifetimeScope lifetimeScope,
      ILogger<AzureServiceBusClient> logger)
    {
      this.topic = topic;
      this.subscription = subscription;
      this.azureServiceBusClientConnection = azureServiceBusClientConnection;
      this.subscriptionManager = subscriptionManager;
      this.lifetimeScope = lifetimeScope;
      this.logger = logger;

      this.sender = azureServiceBusClientConnection.TopicClient.CreateSender(topic);
      ServiceBusProcessorOptions options = new ServiceBusProcessorOptions { MaxConcurrentCalls = 10, AutoCompleteMessages = false };
      this.processor = azureServiceBusClientConnection.TopicClient.CreateProcessor(topic, subscription, options);

      RegisterMessageHandler().GetAwaiter().GetResult();
    }

    public async Task Publish(Message message)
    {
      var eventName = message.GetType().Name;
      var jsonMessage = JsonSerializer.Serialize(message, message.GetType());
      var body = Encoding.UTF8.GetBytes(jsonMessage);

      var serviceBusMessage = new ServiceBusMessage
      {
        MessageId = Guid.NewGuid().ToString(),
        Body = new BinaryData(body),
        Subject = eventName
      };

      await sender.SendMessageAsync(serviceBusMessage);
    }

    public void Subscribe<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>
    {
      var messageTypeName = typeof(T).Name;
      subscriptionManager.AddSubscription<T, TH>();
    }

    public void Unsubscribe<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>
    {
      throw new System.NotImplementedException();
    }

    private Task ExceptionReceivedHandler(ProcessErrorEventArgs args)
    {
      var exception = args.Exception;
      var context = args.ErrorSource;

      logger.LogError(exception, "Failed to handle message: {ExceptionMessage} - Context {@ExceptionContext}", exception.Message, context);

      return Task.CompletedTask;
    }

    private async Task<bool> ProcessMessage(string messageName, string messageBody)
    {
      var isProcessed = false;

      if (subscriptionManager.IsSubscribedToMessage(messageName))
      {
        using var scope = lifetimeScope.BeginLifetimeScope("azure_service_bus");

        var subscriptions = subscriptionManager.GetSubscriptionsForMessage(messageName);

        foreach (var subscription in subscriptions)
        {
          var handler = scope.ResolveOptional(subscription.Handler);

          if (handler == null)
          {
            throw new NullReferenceException();
          }

          var messageType = subscriptionManager.GetMessageTypeByName(messageName);
          var message = JsonSerializer.Deserialize(messageBody, messageType);

          using (LogContext.PushProperty("MessageId", ((Message)message).Id))
          {
            var concreteHandlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);
            var handleMethod = concreteHandlerType.GetMethod("Handle");
            await (Task)handleMethod.Invoke(handler, new object[] { message });
          }
        }

        isProcessed = true;
      }

      return isProcessed;
    }

    private async Task RegisterMessageHandler()
    {
      processor.ProcessMessageAsync +=
        async (args) =>
        {
          using (LogContext.PushProperty("ServiceBusMessageId", args.Message.MessageId))
          {
            var messageName = args.Message.Subject;
            var messageBody = args.Message.Body.ToString();

            if (await ProcessMessage(messageName, messageBody))
            {
              await args.CompleteMessageAsync(args.Message);
            }
          }
        };

      processor.ProcessErrorAsync += ExceptionReceivedHandler;

      await processor.StartProcessingAsync();
    }
  }
}
