namespace JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate
{
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class RequestStatus : Enumeration<RequestStatus, int>
  {
    public static readonly RequestStatus InProgress = new ("In Progress", 1);
    public static readonly RequestStatus Succeeded = new ("Succeeded", 2);
    public static readonly RequestStatus Failed = new ("Failed", 3);

    private RequestStatus(string name, int value)
      : base(name, value)
    {
    }
  }
}
