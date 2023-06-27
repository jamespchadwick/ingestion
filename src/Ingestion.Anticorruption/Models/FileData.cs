namespace JamesPChadwick.Ingestion.Models
{
  using System;

  public class FileData
  {
    public string? Name { get; set; }

    public string? Hash { get; set; }

    public string? Path { get; set; }

    public long? Size { get; set; }

    public DateTimeOffset? CreatedOnUtc { get; set; }
  }
}
