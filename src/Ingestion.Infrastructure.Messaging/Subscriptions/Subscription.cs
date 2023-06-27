namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Subscriptions
{
  using System;

  public class Subscription
  {
    public Subscription(Type handler)
    {
      Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Type Handler { get; }
  }
}
