namespace JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate
{
  using System;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class Message : Entity, IAggregate
  {
    public Message(
      Guid guid,
      string type,
      string payload,
      DateTimeOffset createdOnUtc)
      : base()
    {
      Guid = guid;
      Status = MessageStatus.Unpublished;
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
      CreatedOnUtc = createdOnUtc;
      TimesSent = 0;
    }

    public Guid Guid { get; private set; }

    public Guid? Scope { get; private set; }

    public MessageStatus Status { get; private set; }

    public string Type { get; private set; }

    public string Payload { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public int TimesSent { get; private set; }

    public void SetStatus(MessageStatus status)
    {
      Status = status;

      if (status == MessageStatus.InProgress)
      {
        TimesSent++;
      }
    }

    public void SetScope(Guid scope)
    {
      Scope = scope;
    }
 }
}
