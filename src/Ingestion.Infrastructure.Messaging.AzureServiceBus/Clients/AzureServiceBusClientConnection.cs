namespace JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients.AzureServiceBus
{
  using System;
  using System.Threading.Tasks;
  using Azure.Identity;
  using Azure.Messaging.ServiceBus;

  public class AzureServiceBusClientConnection : IAsyncDisposable
  {
    private readonly string fullyQualifiedNamespace;

    private ServiceBusClient topicClient;

    private bool disposed;

    public AzureServiceBusClientConnection(string fullyQualifiedNamespace)
    {
      this.fullyQualifiedNamespace = fullyQualifiedNamespace;

      topicClient = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
    }

    public ServiceBusClient TopicClient
    {
      get
      {
        if (topicClient.IsClosed)
        {
          topicClient = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
        }

        return topicClient;
      }
    }

    public async ValueTask DisposeAsync()
    {
      if (disposed)
      {
        return;
      }

      disposed = true;
      await topicClient.DisposeAsync();
    }
  }
}
