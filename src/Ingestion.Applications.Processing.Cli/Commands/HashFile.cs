namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class HashFile : IRequest<Unit>
  {
    public string? FilePath { get; set; }
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
      this.fileHashingService = fileHashingService;
      this.fileRepository = fileRepository;
      this.messagingService = messagingService;
      this.logger = logger;
    }

    public async Task<Unit> Handle(HashFile command, CancellationToken cancellationToken)
    {
      var file = await fileRepository.FindByPath(command.FilePath ?? throw new NullReferenceException());

      file.Hash = await fileHashingService.CalculateHash(file);

      fileRepository.Update(file);

      await fileRepository.UnitOfWork.SaveEntitiesAsync();

      return Unit.Value;
    }
  }
}
