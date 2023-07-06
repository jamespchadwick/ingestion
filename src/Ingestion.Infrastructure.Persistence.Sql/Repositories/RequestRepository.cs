namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.Repositories
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using JamesPChadwick.Ingestion.Domain.Seedwork;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts;
  using Microsoft.EntityFrameworkCore;

  public class RequestRepository : IRequestRepository
  {
    private readonly IngestionDbContext dbContext;

    public RequestRepository(IngestionDbContext dbContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<Request> Add(Request requestLogEntry)
    {
      Request entity;

      if (requestLogEntry.IsTransient)
      {
        entity = dbContext.Add(requestLogEntry).Entity;
      }
      else
      {
        entity = requestLogEntry;
      }

      await dbContext.SaveChangesAsync();

      return entity;
    }

    public async Task<Request?> FindByIdempotencyKey(string idempotencyKey)
    {
      return await dbContext.Requests.SingleOrDefaultAsync(request => request.IdempotencyKey == idempotencyKey);
    }

    public async Task Update(Request request)
    {
      dbContext.Entry(request).State = EntityState.Modified;

      await dbContext.SaveChangesAsync();
    }
  }
}