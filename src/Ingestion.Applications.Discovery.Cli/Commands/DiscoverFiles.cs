namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Commands
{
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Discovery.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class DiscoverFiles : IRequest<Unit>
  {
  }

  public class DiscoverFilesHandler : IRequestHandler<DiscoverFiles, Unit>
  {
    private readonly IFileDiscoveryService fileDiscoveryService;
    private readonly IFileRepository fileRepository;
    private readonly IMessagingService messagingService;
    private readonly ILogger<DiscoverFilesHandler> logger;

    public DiscoverFilesHandler(
      IFileDiscoveryService fileDiscoveryService,
      IFileRepository fileRepository,
      IMessagingService messagingService,
      ILogger<DiscoverFilesHandler> logger)
    {
      this.fileDiscoveryService = fileDiscoveryService;
      this.fileRepository = fileRepository;
      this.messagingService = messagingService;
      this.logger = logger;
    }

    public async Task<Unit> Handle(DiscoverFiles command, CancellationToken cancellationToken)
    {
      var discoveredFiles = await fileDiscoveryService.DiscoverFiles();

      foreach (var discoveredFile in discoveredFiles)
      {
        var file = new File(
          discoveredFile.Name ?? string.Empty,
          string.Empty,
          discoveredFile.Path ?? string.Empty,
          discoveredFile.Size,
          discoveredFile.CreatedOnUtc ?? System.DateTimeOffset.UtcNow);

        file = fileRepository.Add(file);

        messagingService.QueueMessageAsync(new FileDiscovered
        {
          FileData = new Models.FileData
          {
            Name = file.Name,
            Path = file.Path,
            Size = file.Size,
            CreatedOnUtc = file.CreatedOnUtc
          }
        });
      }

      await fileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
