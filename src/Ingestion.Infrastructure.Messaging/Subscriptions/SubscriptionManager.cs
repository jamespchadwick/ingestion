namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Subscriptions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;

  public class SubscriptionManager
  {
    private readonly Dictionary<string, List<Subscription>> subscriptions;
    private readonly List<Type> messageTypes;

    public SubscriptionManager()
    {
      subscriptions = new Dictionary<string, List<Subscription>>();
      messageTypes = new List<Type>();
    }

    public virtual bool IsEmpty => !subscriptions.Keys.Any();

    public virtual void AddSubscription<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>
    {
      var messageTypeName = GetMessageTypeName<T>();
      AddSubscription(messageTypeName, typeof(TH));

      if (!messageTypes.Contains(typeof(T)))
      {
        messageTypes.Add(typeof(T));
      }
    }

    public virtual void Clear()
    {
      subscriptions.Clear();
    }

    public virtual Type GetMessageTypeByName(string messageTypeName)
    {
      return messageTypes.Single(type => type.Name == messageTypeName);
    }

    public virtual IEnumerable<Subscription> GetSubscriptionsForMessage<T>()
      where T : Message
    {
      var messageTypeName = GetMessageTypeName<T>();
      return GetSubscriptionsForMessage(messageTypeName);
    }

    public virtual IEnumerable<Subscription> GetSubscriptionsForMessage(string messageTypeName)
    {
      if (!IsSubscribedToMessage(messageTypeName))
      {
        throw new ArgumentException($"Message '{messageTypeName}' has no registered handlers");
      }

      return subscriptions[messageTypeName];
    }

    public virtual bool IsSubscribedToMessage<T>()
      where T : Message
    {
      var messageTypeName = GetMessageTypeName<T>();
      return IsSubscribedToMessage(messageTypeName);
    }

    public virtual bool IsSubscribedToMessage(string messageTypeName)
    {
      return subscriptions.ContainsKey(messageTypeName);
    }

    public virtual void RemoveSubscription<T, TH>()
      where T : Message
      where TH : IMessageHandler<T>
    {
      var messageTypeName = GetMessageTypeName<T>();
      var subscription = FindSubscription(messageTypeName, typeof(TH));

      if (subscription != null)
      {
        RemoveSubscription(messageTypeName, subscription);
      }
    }

    private static string GetMessageTypeName<T>()
    {
      return typeof(T).Name;
    }

    private void AddSubscription(string messageTypeName, Type handler)
    {
      if (!IsSubscribedToMessage(messageTypeName))
      {
        subscriptions.Add(messageTypeName, new List<Subscription>());
      }

      if (subscriptions[messageTypeName].Any(subscription => subscription.Handler == handler))
      {
        throw new ArgumentException($"Handler '{handler.Name}' already registered for message '{messageTypeName}'");
      }

      subscriptions[messageTypeName].Add(new Subscription(handler));
    }

    private Subscription? FindSubscription(string messageTypeName, Type handler)
    {
      if (!IsSubscribedToMessage(messageTypeName))
      {
        return null;
      }

      return subscriptions[messageTypeName].SingleOrDefault(subscription => subscription.Handler == handler);
    }

    private void RemoveSubscription(string messageTypeName, Subscription subscription)
    {
      subscriptions[messageTypeName].Remove(subscription);

      if (!subscriptions[messageTypeName].Any())
      {
        subscriptions.Remove(messageTypeName);

        var messageType = messageTypes.SingleOrDefault(e => e.Name == messageTypeName);
        if (messageType != null)
        {
          messageTypes.Remove(messageType);
        }
      }
    }
  }
}
