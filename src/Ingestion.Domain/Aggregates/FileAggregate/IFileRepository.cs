namespace JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate
{
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public interface IFileRepository : IRepository<File>
  {
    File Add(File file);

    Task<File> FindByPath(string path);

    void Update(File file);
  }
}
