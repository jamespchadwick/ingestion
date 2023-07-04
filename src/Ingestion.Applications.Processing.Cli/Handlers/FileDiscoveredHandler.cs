namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Handlers
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Messages;
  using Microsoft.Extensions.Logging;

  public class FileDiscoveredHandler : IMessageHandler<FileDiscovered>
  {
    private readonly IFileHashingService fileHashingService;
    private readonly IFileRepository fileRepository;
    private readonly IMessagingService messagingService;
    private readonly ILogger<FileDiscoveredHandler> logger;

    public FileDiscoveredHandler(
      IFileHashingService fileHashingService,
      IFileRepository fileRepository,
      IMessagingService messagingService,
      ILogger<FileDiscoveredHandler> logger)
    {
      this.fileHashingService = fileHashingService ?? throw new ArgumentNullException(nameof(fileHashingService));
      this.fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
      this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(FileDiscovered message)
    {
      var file = await fileRepository.FindByPath(message.FileData?.Path ?? throw new NullReferenceException());

      file.Hash = await fileHashingService.CalculateHash(file);

      fileRepository.Update(file);

      messagingService.QueueMessageAsync(new FileHashed
      {
          FileData = new Models.FileData
          {
            Name = file.Name,
            Hash = file.Hash,
            Path = file.Path,
            Size = file.Size,
            CreatedOnUtc = file.CreatedOnUtc
          }
      });

      await fileRepository.UnitOfWork.SaveEntitiesAsync();
    }
  }
}
