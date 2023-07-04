namespace JamesPChadwick.Ingestion.Applications.Discovery.Cli.Behaviors
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using MediatR;
  using Microsoft.Extensions.Logging;
  using Serilog.Context;

  public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
  {
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
      var requestType = request.GetType();
      var requestTypeName = GetRequestTypeName(requestType);

      using (LogContext.PushProperty("RequestType", requestTypeName))
      {
        logger.LogInformation($"START {requestTypeName}");
        var response = await next();
        logger.LogInformation($"END {requestTypeName}");

        return response;
      }
    }

    private string GetRequestTypeName(Type requestType)
    {
      if (requestType.IsGenericType)
      {
        var genericTypes = string.Join(",", requestType.GetGenericArguments().Select(t => t.Name).ToArray());
        return $"{requestType.Name.Remove(requestType.Name.IndexOf('`'))}<{genericTypes}>";
      }
      else
      {
        return requestType.Name;
      }
    }
  }
}
