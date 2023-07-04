namespace JamesPChadwick.Ingestion.Applications.Processing.Cli
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Threading.Tasks;
  using Autofac;
  using Autofac.Extensions.DependencyInjection;
  using Azure.Identity;
  using JamesPChadwick.Ingestion.Applications.Processing.Cli.Modules;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Clients;
  using JamesPChadwick.Ingestion.Infrastructure.Messaging.Messages;
  using JamesPChadwick.Ingestion.Messages;
  using MediatR;
  using Microsoft.ApplicationInsights.Extensibility;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Serilog;
  using Serilog.Events;
  using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

  public class Program
  {
    public static readonly string ApplicationName = "Ingestion-Processing-Cli";
    public static readonly string? ApplicationVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

    public static void Main(string[] args)
    {
      var configuration = BuildConfiguration(args);

      Log.Logger = CreateLogger(configuration);

      try
      {
        Log.Information("Starting {ApplicationName}-{ApplicationVersion}", ApplicationName, ApplicationVersion);

        var host = BuildHost(configuration);

        var lifetimeScope = host.Services.GetRequiredService<ILifetimeScope>();

        using var scope = lifetimeScope.BeginLifetimeScope(ApplicationName);
        {
          var messagingClient = scope.Resolve<IMessagingClient>();
          messagingClient.Subscribe<FileDiscovered, IMessageHandler<FileDiscovered>>();

          Console.ReadKey();
        }
      }
      catch (Exception exception)
      {
        Log.Fatal(exception, "{ApplicationName}-{ApplicationVersion} unexpectedly terminated", ApplicationName, ApplicationVersion);
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    private static IConfiguration BuildConfiguration(string[] args)
    {
      var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

      if (args != null)
      {
        builder.AddCommandLine(args);
      }

      var configuration = builder.Build();

      builder.AddAzureKeyVault(
        new Uri(configuration["Azure:KeyVault:VaultUri"] ?? throw new ArgumentNullException("Azure:KeyVault:VaultUri")),
        new DefaultAzureCredential());

      return builder.Build();
    }

    private static IHost BuildHost(IConfiguration configuration)
    {
      HostBuilder builder = new ();

      builder.UseContentRoot(Directory.GetCurrentDirectory())
             .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
             .UseSerilog()
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
             .ConfigureServices((services) =>
             {
                services
                  .AddMediator(configuration)
                  .AddDbContext(configuration);
             })
             .ConfigureContainer<ContainerBuilder>(builder =>
             {
                builder.RegisterModule(new ApplicationModule(configuration));
                builder.RegisterModule(new BlobStorageModule(configuration));
                builder.RegisterModule(new MessagingModule(configuration));
                builder.RegisterModule(new PersistenceModule(configuration));
             });

      return builder.Build();
    }

    private static ILogger CreateLogger(IConfiguration configuration)
    {
      var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];

      return new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .Enrich.WithProperty("ApplicationName", ApplicationName ?? string.Empty)
                 .Enrich.WithProperty("ApplicationVersion", ApplicationVersion ?? string.Empty)
                 .Enrich.FromLogContext()
                 .Enrich.WithMachineName()
                 .Enrich.WithEnvironmentName()
                 .WriteTo.Console()
                 .WriteTo.ApplicationInsights(
                   new TelemetryConfiguration { InstrumentationKey = instrumentationKey },
                   new TraceTelemetryConverter(),
                   LogEventLevel.Information)
                 .CreateLogger();
    }
  }
}
