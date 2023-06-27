namespace JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate
{
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class FileContainer : Enumeration<FileContainer>
  {
    public static readonly FileContainer FileDrop = new ("file-drop");

    private FileContainer(string name)
      : base(name)
    {
    }
  }
}
