namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Commands
{
  using System;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Exceptions;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Services;
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using MediatR;
  using Serilog.Context;

  public class IdempotentCommand<TRequest, TResponse> : IRequest<TResponse>
    where TRequest : IRequest<TResponse>
  {
    public IdempotentCommand(string idempotencyKey, TRequest command)
    {
      IdempotencyKey = idempotencyKey;
      Command = command;
    }

    public string IdempotencyKey { get; init; }

    public TRequest Command { get; init; }
  }

  public abstract class IdempotentCommandHandler<TRequest, TResponse> : IRequestHandler<IdempotentCommand<TRequest, TResponse>, TResponse>
    where TRequest : IRequest<TResponse>
  {
    private readonly IIdempotencyService idempotencyService;
    private readonly IMediator mediator;

    public IdempotentCommandHandler(
      IIdempotencyService idempotencyService,
      IMediator mediator)
    {
      this.idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
      this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TResponse> Handle(IdempotentCommand<TRequest, TResponse> request, CancellationToken cancellationToken)
    {
      var existingRequest = await idempotencyService.GetRequest(request.IdempotencyKey);

      if (existingRequest is not null)
      {
        if (existingRequest.Status == RequestStatus.InProgress)
        {
          throw new IdempotencyException($"Processing for request `{request.IdempotencyKey}` is currently in progress");
        }
        else if (existingRequest.Status == RequestStatus.Succeeded)
        {
          return CreateResponseForDuplicateRequest(request.IdempotencyKey, request.Command, existingRequest);
        }
      }

      await idempotencyService.LogRequest(
        request.IdempotencyKey,
        request.GetType().Name,
        JsonSerializer.Serialize(request.Command, request.Command.GetType()),
        DateTimeOffset.UtcNow);

      TResponse response;

      try
      {
        using (LogContext.PushProperty("IdempotencyKey", request.IdempotencyKey))
        {
          response = await mediator.Send(request.Command, cancellationToken);

          await idempotencyService.RecordResult(
            request.IdempotencyKey,
            true,
            JsonSerializer.Serialize(response, response!.GetType()));

          return response;
        }
      }
      catch (Exception exception)
      {
        await idempotencyService.RecordResult(request.IdempotencyKey, false, exception.Message);
        return default!;
      }
    }

    protected virtual TResponse CreateResponseForDuplicateRequest(string idempotencyKey, TRequest request, Request requestAggregate)
    {
      throw new NotImplementedException();
    }
  }
}