namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Modules
{
  using System;
  using System.Data.Common;
  using System.Data.SqlClient;
  using Autofac;
  using JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.Repositories;
  using Microsoft.Extensions.Configuration;

  public class PersistenceModule : Autofac.Module
  {
    private readonly IConfiguration configuration;

    public PersistenceModule(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(c => new SqlConnection(configuration["SqlServer:ConnectionString"]))
        .As<DbConnection>()
        .InstancePerLifetimeScope();

      builder.RegisterType<FileRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
      builder.RegisterType<MessageRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    }
  }
}
