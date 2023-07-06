namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Handlers
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class FileHashedHandler : IMessageHandler<FileHashed>
  {
    private readonly IMediator mediator;
    private readonly ILogger<FileHashedHandler> logger;

    public FileHashedHandler(
      IMediator mediator,
      ILogger<FileHashedHandler> logger)
    {
      this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(FileHashed message)
    {
      var validateFileCommand = new ValidateFile { FilePath = message?.FileData?.Path };
      var idempotentValidateFileCommand = new IdempotentCommand<ValidateFile, Unit>(message?.FileData?.Hash ?? Guid.NewGuid().ToString(), validateFileCommand);
      await mediator.Send(idempotentValidateFileCommand);
    }
  }
}
