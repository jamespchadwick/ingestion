namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Handlers
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class FileDiscoveredHandler : IMessageHandler<FileDiscovered>
  {
    private readonly IMediator mediator;
    private readonly ILogger<FileDiscoveredHandler> logger;

    public FileDiscoveredHandler(
      IMediator mediator,
      ILogger<FileDiscoveredHandler> logger)
    {
      this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(FileDiscovered message)
    {
      var hashFileCommand = new HashFile
      {
        FileData = new Models.FileData
        {
          Name = message.FileData?.Name,
          Hash = message.FileData?.Hash,
          Path = message.FileData?.Path,
          Size = message.FileData?.Size,
          CreatedOnUtc = message.FileData?.CreatedOnUtc
        }
      };

      await mediator.Send(hashFileCommand);
    }
  }
}
