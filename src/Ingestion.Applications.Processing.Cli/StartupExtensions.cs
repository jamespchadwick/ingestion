namespace JamesPChadwick.Ingestion.Applications.Processing.Cli
{
  using System;
  using System.Data.Common;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Behaviors;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.DbContexts;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Conventions;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  public static class StartupExtensions
  {
    public static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration configuration)
    {
      return services.AddMediatR(configuration =>
      {
        configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
      });
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<IngestionDbContext>((serviceProvider, dbContextOptionsBuilder) =>
      {
        var modelBuilder = SqlServerConventionSetBuilder.CreateModelBuilder();
        modelBuilder.HasDefaultSchema("ingestion");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IngestionDbContext).Assembly);
        var model = modelBuilder.FinalizeModel();

        dbContextOptionsBuilder
          .UseModel(model)
          .UseSqlServer(
            serviceProvider.GetRequiredService<DbConnection>(),
            sqlServerOptions =>
            {
              sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
      });

      return services;
    }
  }
}
