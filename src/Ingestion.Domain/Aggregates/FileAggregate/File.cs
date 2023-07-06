namespace JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate
{
  using System;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class File : Entity, IAggregate
  {
    public File(
      string name,
      string hash,
      string path,
      long? size,
      DateTimeOffset createdOnUtc)
      : base()
    {
      Name = name;
      Hash = hash;
      Path = path;
      Size = size;
      CreatedOnUtc = createdOnUtc;
    }

    public string Name { get; set; }

    public string Hash { get; set; }

    public string Path { get; set; }

    public long? Size { get; set; }

    public DateTimeOffset CreatedOnUtc { get; set; }

    public bool IsPartialFile => Name.ToUpper().Contains("FILEPART");
   }
}
