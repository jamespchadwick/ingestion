namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Services
{
  using System;
  using System.Threading.Tasks;
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using Microsoft.Extensions.Logging;

  public interface IIdempotencyService
  {
    Task<Request?> GetRequest(string idempotencyKey);

    Task LogRequest(string requestId, string requestType, string payload, DateTimeOffset timeStamp);

    Task RecordResult(string idempotencyKey, bool isSuccess, string message);
  }

  public class IdempotencyService : IIdempotencyService
  {
    private readonly IRequestRepository requestRepository;
    private readonly ILogger<IdempotencyService> logger;

    public IdempotencyService(
      IRequestRepository requestRepository,
      ILogger<IdempotencyService> logger)
    {
      this.requestRepository = requestRepository;
      this.logger = logger;
    }

    public async Task<Request?> GetRequest(string idempotencyKey)
    {
      return await requestRepository.FindByIdempotencyKey(idempotencyKey);
    }

    public virtual async Task LogRequest(string idempotencyKey, string requestType, string payload, DateTimeOffset timeStamp)
    {
      var request = await requestRepository.FindByIdempotencyKey(idempotencyKey);

      if (request is null)
      {
        request = new Request(idempotencyKey, requestType, payload, timeStamp);
        request = await requestRepository.Add(request);
      }
    }

    public virtual async Task RecordResult(string idempotencyKey, bool isSuccess, string message)
    {
      var request = await requestRepository.FindByIdempotencyKey(idempotencyKey);

      if (request is not null)
      {
        request.RecordResult(isSuccess, message, DateTimeOffset.UtcNow);
        await requestRepository.Update(request);
      }
    }
  }
}
