namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Modules
{
  using System;
  using System.Reflection;
  using Autofac;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using Microsoft.Extensions.Configuration;

  public class ApplicationModule : Autofac.Module
  {
    private readonly IConfiguration configuration;

    public ApplicationModule(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<BlobFileHashingService>().As<IFileHashingService>();
      builder.RegisterType<MessagingService>().As<IMessagingService>();

      builder.RegisterAssemblyTypes(typeof(Program).GetTypeInfo().Assembly)
        .AsClosedTypesOf(typeof(IMessageHandler<>));
    }
  }
}
