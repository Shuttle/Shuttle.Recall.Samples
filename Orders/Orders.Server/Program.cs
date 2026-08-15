using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orders.Data;
using Orders.Server.EventHandlers;
using Serilog;
using Shuttle.Reflection;
using Shuttle.Threading;
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
                    .AddThreading(options =>
                    {
                        options.ProcessorException += (eventArgs, _) =>
                        {
                            Console.WriteLine($"[ProcessorException] : service key = '{eventArgs.ProcessorThread.ServiceKey}' / exception = '{eventArgs.Exception?.AllMessages() ?? "null"}'");

                            return Task.CompletedTask;
                        };
                    })
                    .Services
                    .AddRecall(options =>
                    {
                        options.EventStore.PrimitiveEventSequencerIdleDurations = [TimeSpan.FromSeconds(1)];
                        options.EventProcessing.ProjectionProcessorIdleDurations = [TimeSpan.FromSeconds(1)];
                    })
                    .UseSqlServerEventStorage(options =>
                    {
                        options.ConnectionString = context.Configuration.GetConnectionString("Storage") ?? throw new ApplicationException("A 'ConnectionString' with name 'Storage' is required which points to a Sql Server database that will contain the event storage.");
                        options.Schema = "recall_samples";
                    })
                    .RegisterPrimitiveEventSequencing()
                    .UseSqlServerEventProcessing()
                    .AddProjection<OrderHandler>("orders")
                    .Services
                    .AddHopper(options =>
                    {
                        context.Configuration.GetSection(HopperOptions.SectionName).Bind(options);
                    })
                    .UseAzureStorageQueues(builder =>
                    {
                        builder.Configure("recall-samples", options =>
                        {
                            options.ConnectionString = context.Configuration.GetConnectionString("Azurite")!;
                        });
                    })
                    .AddMessageHandlersFrom(typeof(Program).Assembly)
                    .Services
                    .AddOrderData();
            })
            .Build()
            .RunAsync();
    }
}