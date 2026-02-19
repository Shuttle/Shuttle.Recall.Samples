using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orders.Data;
using Orders.Server.EventHandlers;
using Serilog;
using Shuttle.Core.Reflection;
using Shuttle.Core.Threading;
using Shuttle.Hopper;
using Shuttle.Hopper.AzureStorageQueues;
using Shuttle.Recall;
using Shuttle.Recall.SqlServer.EventProcessing;
using Shuttle.Recall.SqlServer.Storage;

namespace Orders.Server;

internal class Program
{
    private static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.Sources.Clear();
                configurationBuilder
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables();
            })
            .UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddThreading(builder =>
                    {
                        builder.ConfigureThreading(options =>
                        {
                            options.ProcessorException += (eventArgs, _) =>
                            {
                                Console.WriteLine($"[ProcessorException] : service key = '{eventArgs.ProcessorThread.ServiceKey}' / exception = '{eventArgs.Exception?.AllMessages() ?? "null"}'");

                                return Task.CompletedTask;
                            };
                        });
                    })
                    .AddRecall(recallBuilder =>
                    {
                        recallBuilder.Options.EventStore.PrimitiveEventSequencerIdleDurations = [TimeSpan.FromSeconds(1)];
                        recallBuilder.Options.EventProcessing.ProjectionProcessorIdleDurations = [TimeSpan.FromSeconds(1)];

                        recallBuilder
                            .UseSqlServerEventStorage(builder =>
                            {
                                builder.Options.ConnectionString = context.Configuration.GetConnectionString("StorageConnection") ?? throw new ApplicationException("A 'ConnectionString' with name 'StorageConnection' is required which points to a Sql Server database that will contain the event storage.");
                                builder.Options.Schema = "recall_samples";
                            })
                            .UseSqlServerEventProcessing(builder =>
                            {
                                builder.Options.ConnectionString = context.Configuration.GetConnectionString("EventProcessingConnection") ?? throw new ApplicationException("A 'ConnectionString' with name 'EventProcessingConnection' is required which points to a Sql Server database that will contain the projections.");
                                builder.Options.Schema = "recall_samples";
                                builder.Options.DbConnectionServiceKey = "Orders";
                            })
                            .AddProjection("orders").AddEventHandler<OrderHandler>();
                    })
                    .AddHopper(hopperBuilder =>
                    {
                        context.Configuration.GetSection(HopperOptions.SectionName).Bind(hopperBuilder.Options);

                        hopperBuilder
                            .UseAzureStorageQueues(builder =>
                            {
                                builder.AddOptions("recall-samples", new()
                                {
                                    ConnectionString = context.Configuration.GetConnectionString("Azurite")!
                                });
                            });
                    })
                    .AddOrderData();
            })
            .Build()
            .RunAsync();
    }
}