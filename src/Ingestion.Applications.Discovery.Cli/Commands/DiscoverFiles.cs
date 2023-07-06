namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Commands
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Discovery.Cli.Services;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class DiscoverFiles : IRequest<Unit>
  {
  }

  public class DiscoverFilesHandler : IRequestHandler<DiscoverFiles, Unit>
  {
    private readonly IFileDiscoveryService fileDiscoveryService;
    private readonly IMessagingService messagingService;
    private readonly ILogger<DiscoverFilesHandler> logger;

    public DiscoverFilesHandler(
      IFileDiscoveryService fileDiscoveryService,
      IMessagingService messagingService,
      ILogger<DiscoverFilesHandler> logger)
    {
      this.fileDiscoveryService = fileDiscoveryService;
      this.messagingService = messagingService;
      this.logger = logger;
    }

    public async Task<Unit> Handle(DiscoverFiles command, CancellationToken cancellationToken)
    {
      var discoveredFiles = await fileDiscoveryService.DiscoverFiles();

      Guid batch = Guid.NewGuid();

      foreach (var discoveredFile in discoveredFiles)
      {
        await messagingService.QueueMessageAsync(
          batch,
          new FileDiscovered
          {
            FileData = new Models.FileData
            {
              Name = discoveredFile.Name ?? string.Empty,
              Path = discoveredFile.Path ?? string.Empty,
              Size = discoveredFile.Size,
              CreatedOnUtc = discoveredFile.CreatedOnUtc ?? System.DateTimeOffset.UtcNow
            }
          },
          cancellationToken);
      }

      await messagingService.PublishMessages(batch, cancellationToken);

      return Unit.Value;
    }
  }
}
