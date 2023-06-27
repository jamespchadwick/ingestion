namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Behaviors
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Discovery.Cli.Services;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts;
  using MediatR;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using Serilog.Context;

  public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
  {
    private readonly IngestionDbContext ingestionDbContext;
    private readonly IMessagingService messagingService;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> logger;

    public TransactionBehavior(
        IngestionDbContext ingestionDbContext,
        IMessagingService messagingService,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
      this.ingestionDbContext = ingestionDbContext ?? throw new ArgumentNullException(nameof(ingestionDbContext));
      this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
      var response = default(TResponse);

      try
      {
        if (ingestionDbContext.HasActiveTransaction)
        {
          return await next();
        }

        var executionStrategy = ingestionDbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
          Guid transactionId;

          using (var transaction = await ingestionDbContext.BeginTransactionAsync())
          using (LogContext.PushProperty("TransactionId", transaction.TransactionId))
          {
            logger.LogInformation("Begin transaction {TransactionId}", transaction.TransactionId);
            response = await next();
            await ingestionDbContext.CommitTransactionAsync(transaction);
            logger.LogInformation("Commit transaction {TransactionId}", transaction.TransactionId);
            transactionId = transaction.TransactionId;
          }

          await messagingService.PublishMessages(transactionId);
        });

        return response ?? throw new NullReferenceException();
      }
      catch (Exception exception)
      {
        logger.LogError(exception, $"Rollback transaction: {exception.Message}");
        throw;
      }
    }
  }
}
