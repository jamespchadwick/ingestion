namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages
{
  using System;
  using System.Text.Json.Serialization;

  public class Message
  {
    public Message()
    {
      Id = Guid.NewGuid();
      TimeStamp = DateTimeOffset.UtcNow;
    }

    [JsonConstructor]
    public Message(Guid id, DateTimeOffset timeStamp)
    {
      Id = id;
      TimeStamp = timeStamp;
    }

    [JsonInclude]
    public Guid Id { get; private set; }

    [JsonInclude]
    public DateTimeOffset TimeStamp { get; private set; }
  }
}
