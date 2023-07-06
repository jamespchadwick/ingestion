namespace JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate
{
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public interface IRequestRepository : IRepository<Request>
  {
    Task<Request> Add(Request request);

    Task<Request?> FindByIdempotencyKey(string idempotencyKey);

    Task Update(Request request);
  }
}
