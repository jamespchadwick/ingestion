namespace JamesPChadwick.Ingestion.Domain.Seedwork
{
  public interface IRepository<T>
    where T : IAggregate
  {
    IUnitOfWork UnitOfWork { get; }
  }
}
