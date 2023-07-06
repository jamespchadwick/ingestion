namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.Repositories
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Domain.Seedwork;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts;
  using Microsoft.EntityFrameworkCore;

  public class FileRepository : IFileRepository
  {
    private readonly IngestionDbContext dbContext;

    public FileRepository(IngestionDbContext dbContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => dbContext;

    public File Add(File file)
    {
      if (file.IsTransient)
      {
        return dbContext.Files.Add(file).Entity;
      }
      else
      {
        return file;
      }
    }

    public async Task<File?> FindByPath(string path)
    {
      return await dbContext.Files.SingleOrDefaultAsync(file => file.Path == path);
    }

    public void Update(File file)
    {
      if (file.IsTransient)
      {
        return;
      }

      dbContext.Entry(file).State = EntityState.Modified;
    }
  }
}
