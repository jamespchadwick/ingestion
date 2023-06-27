namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Services
{
  using System.IO;
  using System.Security.Cryptography;
  using System.Threading.Tasks;
  using Azure.Storage.Blobs;

  public interface IFileHashingService
  {
    Task<string> CalculateHash(Domain.Aggregates.FileAggregate.File file);
  }

  public class BlobFileHashingService : IFileHashingService
  {
    private readonly BlobContainerClient blobContainerClient;

    public BlobFileHashingService(BlobContainerClient blobContainerClient)
    {
      this.blobContainerClient = blobContainerClient;
    }

    public async Task<string> CalculateHash(Domain.Aggregates.FileAggregate.File file)
    {
      var downloadStream = await blobContainerClient.GetBlobClient(file.Name).OpenReadAsync();

      string hash = string.Empty;

      using (var bufferedStream = new BufferedStream(downloadStream, 1200000))
      using (var sha512 = SHA512.Create())
      {
        var hashValue = sha512.ComputeHash(bufferedStream);

        foreach (byte x in hashValue)
        {
          hash += string.Format("{0:x2}", x);
        }
      }

      return hash;
    }
  }
}
