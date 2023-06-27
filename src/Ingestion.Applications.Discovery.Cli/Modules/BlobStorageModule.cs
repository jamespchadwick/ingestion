namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Modules
{
  using System;
  using Autofac;
  using Azure.Identity;
  using Azure.Storage.Blobs;
  using Microsoft.Extensions.Configuration;

  public class BlobStorageModule : Autofac.Module
  {
    private readonly IConfiguration configuration;

    public BlobStorageModule(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void Load(ContainerBuilder builder)
    {
      var fileDropContainerUri = configuration["Azure:Storage:FileDropContainerUri"]
                               ?? throw new ArgumentNullException("Azure:Storage:FileDropContainerUri");

      var client = new BlobContainerClient(new Uri(fileDropContainerUri), new DefaultAzureCredential());

      builder.RegisterInstance(client).AsSelf();
    }
  }
}
