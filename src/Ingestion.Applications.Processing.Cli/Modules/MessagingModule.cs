namespace JamesPChadwick.Ingestion.Applications.Processing.Cli.Modules
{
  using System;
  using Autofac;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients.AzureServiceBus;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Subscriptions;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;

  public class MessagingModule : Autofac.Module
  {
    private readonly IConfiguration configuration;

    public MessagingModule(IConfiguration configuration)
    {
      this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void Load(ContainerBuilder builder)
    {
      var fullyQualifiedNamespace = configuration["ServiceBus:FullyQualifiedNamespace"]
                                  ?? throw new ArgumentNullException("ServiceBus:FullyQualifiedNamespace");

      var azureServiceBusClientConnection = new AzureServiceBusClientConnection(fullyQualifiedNamespace);
      builder.RegisterInstance(azureServiceBusClientConnection);

      builder.RegisterType<SubscriptionManager>().AsSelf().SingleInstance();

      builder.Register<AzureServiceBusClient>(c =>
             {
                var connection = c.Resolve<AzureServiceBusClientConnection>();
                var subscriptionManager = c.Resolve<SubscriptionManager>();
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                var logger = c.Resolve<ILogger<AzureServiceBusClient>>();

                return new AzureServiceBusClient("messages", Program.ApplicationName, connection, subscriptionManager, lifetimeScope, logger);
             })
             .As<IMessagingClient>()
             .SingleInstance();
    }
  }
}
