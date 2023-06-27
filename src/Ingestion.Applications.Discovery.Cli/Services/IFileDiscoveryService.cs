namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Azure.Storage.Blobs;
  using JamesPChadwick.Ingestion.Models;

  public interface IFileDiscoveryService
  {
    Task<IEnumerable<FileData>> DiscoverFiles();
  }

  public class BlobContainerFileDiscoveryService : IFileDiscoveryService
  {
    private readonly BlobContainerClient blobContainerClient;

    public BlobContainerFileDiscoveryService(BlobContainerClient blobContainerClient)
    {
      this.blobContainerClient = blobContainerClient;
    }

    public async Task<IEnumerable<FileData>> DiscoverFiles()
    {
      var fileDatas = new List<FileData>();

      await foreach (var blob in blobContainerClient.GetBlobsAsync())
      {
        fileDatas.Add(new FileData
        {
          Name = blob.Name,
          Path = $"{blobContainerClient.Uri}/{blob.Name}",
          Size = blob.Properties.ContentLength,
          CreatedOnUtc = blob.Properties.CreatedOn
        });
      }

      return fileDatas;
    }
  }
}
