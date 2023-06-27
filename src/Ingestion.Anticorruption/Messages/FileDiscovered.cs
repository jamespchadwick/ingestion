namespace JamesPChadwick.Ingestion.Messages
{
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Models;

  public class FileDiscovered : Message
  {
    public FileData? FileData { get; set; }
  }
}
