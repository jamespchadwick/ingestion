namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Messages;
  using JamesPChadwick.Ingestion.Models;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class HashFile : IRequest<Unit>
  {
    public FileData? FileData { get; set; }
  }

  public class HashFileHandler : IRequestHandler<HashFile, Unit>
  {
    private readonly IFileHashingService fileHashingService;
    private readonly IFileRepository fileRepository;
    private readonly IMessagingService messagingService;
    private readonly ILogger<HashFileHandler> logger;

    public HashFileHandler(
      IFileHashingService fileHashingService,
      IFileRepository fileRepository,
      IMessagingService messagingService,
      ILogger<HashFileHandler> logger)
    {
      this.fileHashingService = fileHashingService ?? throw new ArgumentNullException(nameof(fileHashingService));
      this.fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
      this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(HashFile command, CancellationToken cancellationToken)
    {
      File? file;

      if (!string.IsNullOrWhiteSpace(command.FileData?.Path))
      {
        file = await fileRepository.FindByPath(command.FileData.Path);

        if (file is not null)
        {
          return Unit.Value;
        }
      }

      file = new File(
        command.FileData?.Name ?? string.Empty,
        string.Empty,
        command.FileData?.Path ?? string.Empty,
        command.FileData?.Size,
        command.FileData?.CreatedOnUtc ?? System.DateTimeOffset.UtcNow);

      file.Hash = await fileHashingService.CalculateHash(file);

      file = fileRepository.Add(file);

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

      return Unit.Value;
    }
  }
}
