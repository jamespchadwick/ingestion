namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Exceptions;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using MediatR;
  using Microsoft.Extensions.Logging;

  public class ValidateFile : IRequest<Unit>
  {
    public string? FilePath { get; set; }
  }

  public class ValidateFileHandler : IRequestHandler<ValidateFile, Unit>
  {
    private readonly IFileRepository fileRepository;
    private readonly IMessagingService messagingService;
    private readonly ILogger<ValidateFileHandler> logger;

    public ValidateFileHandler(
      IFileRepository fileRepository,
      IMessagingService messagingService,
      ILogger<ValidateFileHandler> logger)
    {
      this.fileRepository = fileRepository;
      this.messagingService = messagingService;
      this.logger = logger;
    }

    public async Task<Unit> Handle(ValidateFile command, CancellationToken cancellationToken)
    {
      var file = await fileRepository.FindByPath(command.FilePath ?? throw new NullReferenceException());

      if (file is null)
      {
        throw new FileNotFoundException();
      }

      if (file.IsPartialFile)
      {
        throw new PartialFileException();
      }

      throw new NotImplementedException();
    }
  }

  public class IdempotentValidateFileHandler : IdempotentCommandHandler<ValidateFile, Unit>
  {
    public IdempotentValidateFileHandler(IIdempotencyService idempotencyService, IMediator mediator)
      : base(idempotencyService, mediator)
    {
    }

    protected override Unit CreateResponseForDuplicateRequest(string requestId, ValidateFile request, Request requestAggregate)
    {
      return Unit.Value;
    }
  }
}
