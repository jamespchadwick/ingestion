namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Exceptions
{
  using System;

  public class PartialFileException : Exception
  {
    public PartialFileException()
      : base()
    {
    }

    public PartialFileException(string message)
      : base(message)
    {
    }

    public PartialFileException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}