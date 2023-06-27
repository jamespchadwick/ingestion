namespace JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate
{
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class MessageStatus : Enumeration<MessageStatus, int>
  {
    public static readonly MessageStatus Unpublished = new ("Unpublished", 1);
    public static readonly MessageStatus InProgress = new ("In Progress", 2);
    public static readonly MessageStatus Published = new ("Published", 3);
    public static readonly MessageStatus Failed = new ("Failed", 4);

    private MessageStatus(string name, int value)
      : base(name, value)
    {
    }
  }
}
