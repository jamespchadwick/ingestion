namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Exceptions
{
  using System;

  public class IdempotencyException : Exception
  {
    public IdempotencyException()
      : base()
    {
    }

    public IdempotencyException(string message)
      : base(message)
    {
    }

    public IdempotencyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}