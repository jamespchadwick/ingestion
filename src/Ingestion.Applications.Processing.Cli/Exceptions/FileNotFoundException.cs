namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Exceptions
{
  using System;

  public class FileNotFoundException : Exception
  {
    public FileNotFoundException()
      : base()
    {
    }

    public FileNotFoundException(string message)
      : base(message)
    {
    }

    public FileNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}