namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IUnitOfWork : IDisposable
  {
    Guid? TransactionId { get; }

    Task SaveEntitiesAsync(CancellationToken cancellationToken = default);
  }
}
