namespace JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate
{
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public class FileType : Enumeration<FileType>
  {
    public static readonly FileType CSV = new (".csv");
    public static readonly FileType TXT = new (".txt");
    public static readonly FileType PGP = new (".pgp");
    public static readonly FileType XLS = new (".xls");
    public static readonly FileType XLSX = new (".xlsx");

    private FileType(string name)
      : base(name)
    {
    }
  }
}
