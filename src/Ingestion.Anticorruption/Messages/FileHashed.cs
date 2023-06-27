namespace JamesPChadwick.Ingestion.Messages
{
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Models;

  public class FileHashed : Message
  {
    public FileData? FileData { get; set; }
  }
}
