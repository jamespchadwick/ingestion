namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  public class DomainException : System.Exception
  {
    public DomainException()
    {
    }

    public DomainException(string message)
      : base(message)
    {
    }

    public DomainException(string message, System.Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
