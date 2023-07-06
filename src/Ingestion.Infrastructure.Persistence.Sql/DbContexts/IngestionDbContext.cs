namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts
{
  using System;
  using System.Data;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate;
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using JamesPChadwick.Ingestion.Domain.Seedwork;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Storage;

  public class IngestionDbContext : DbContext, IUnitOfWork
  {
    private IDbContextTransaction? currentTransaction;

    public IngestionDbContext(DbContextOptions<IngestionDbContext> options)
      : base(options)
    {
    }

    public DbSet<File> Files { get; set; } = null!;

    public DbSet<Message> Messages { get; set; } = null!;

    public DbSet<Request> Requests { get; set; } = null!;

    public bool HasActiveTransaction => currentTransaction != null;

    public Guid? TransactionId => currentTransaction?.TransactionId;

    public async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
      await SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
      currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
      return currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
      if (transaction == null)
      {
        throw new ArgumentNullException(nameof(transaction));
      }

      if (transaction != currentTransaction)
      {
        throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
      }

      try
      {
        await SaveChangesAsync();
        transaction.Commit();
      }
      catch
      {
        RollbackTransaction();
        throw;
      }
      finally
      {
        if (currentTransaction != null)
        {
          currentTransaction.Dispose();
          currentTransaction = null;
        }
      }
    }

    public void RollbackTransaction()
    {
      try
      {
        currentTransaction?.Rollback();
      }
      finally
      {
        if (currentTransaction != null)
        {
          currentTransaction.Dispose();
          currentTransaction = null;
        }
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(IngestionDbContext).Assembly);
    }
  }
}
