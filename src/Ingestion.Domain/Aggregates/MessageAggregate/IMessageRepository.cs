namespace JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Seedwork;

  public interface IMessageRepository : IRepository<Message>
  {
    Message Add(Message message);

    Task<Message> FindByGuid(Guid id);

    Task<IEnumerable<Message>> FindByStatus(MessageStatus status);

    Task<IEnumerable<Message>> FindByStatus(MessageStatus status, Guid scope);

    void Update(Message message);
  }
}
