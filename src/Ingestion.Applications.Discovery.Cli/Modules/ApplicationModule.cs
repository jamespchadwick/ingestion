namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Modules
{
  using System;
  using Autofac;
  using JamesPChadwick.Ingestion.Applications.Discovery.Cli.Services;
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
      builder.RegisterType<BlobContainerFileDiscoveryService>().As<IFileDiscoveryService>();
      builder.RegisterType<MessagingService>().As<IMessagingService>();
    }
  }
}
