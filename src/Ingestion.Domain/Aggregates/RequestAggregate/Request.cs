namespace JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate
{
  using System;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class Request : Entity, IAggregate
  {
    public Request(
      string idempotencyKey,
      string type,
      string payload,
      DateTimeOffset timeStamp)
      : base()
    {
      IdempotencyKey = idempotencyKey;
      Status = RequestStatus.InProgress;
      Type = type;
      Payload = payload;
      TimeStamp = timeStamp;
      TimesProcessed = 0;
    }

    public string IdempotencyKey { get; private set; }

    public RequestStatus Status { get; private set; }

    public string Type { get; private set; }

    public string Payload { get; private set; }

    public DateTimeOffset TimeStamp { get; private set; }

    public int TimesProcessed { get; private set; }

    public bool? IsSuccess { get; set; }

    public string? Message { get; set; }

    public DateTimeOffset? LastProcessedOnUtc { get; set; }

    public void RecordResult(bool isSuccess, string message, DateTimeOffset lastProcessedOn)
    {
      Status = isSuccess ? RequestStatus.Succeeded : RequestStatus.Failed;
      IsSuccess = isSuccess;
      Message = message;
      LastProcessedOnUtc = lastProcessedOn;

      TimesProcessed++;
    }
  }
}