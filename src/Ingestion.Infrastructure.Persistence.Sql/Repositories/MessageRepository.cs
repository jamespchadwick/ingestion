namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate;
  using JamesPChadwick.Ingestion.Domain.Seedwork;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts;
  using Microsoft.EntityFrameworkCore;

  public class MessageRepository : IMessageRepository
  {
    private readonly IngestionDbContext dbContext;

    public MessageRepository(IngestionDbContext dbContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => dbContext;

    public Message Add(Message message)
    {
      if (message.IsTransient)
      {
        return dbContext.Messages.Add(message).Entity;
      }
      else
      {
        return message;
      }
    }

    public async Task<Message> FindByGuid(Guid guid)
    {
      return await dbContext.Messages.SingleAsync(message => message.Guid == guid);
    }

    public async Task<IEnumerable<Message>> FindByStatus(MessageStatus status)
    {
      return await FindMessages(message => message.Status == status);
    }

    public async Task<IEnumerable<Message>> FindByStatus(MessageStatus status, Guid scope)
    {
      return await FindMessages(message => message.Status == status && message.Scope == scope);
    }

    public void Update(Message message)
    {
      dbContext.Entry(message).State = EntityState.Modified;
    }

    private async Task<IEnumerable<Message>> FindMessages(Expression<Func<Message, bool>> prediacte)
    {
      return await dbContext
                   .Messages
                   .Where(prediacte)
                   .OrderBy(messageLogEntry => messageLogEntry.CreatedOnUtc)
                   .ToListAsync();
    }
  }
}
