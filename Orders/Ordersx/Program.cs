using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Data;
using Shuttle.Recall;
using Shuttle.Recall.SqlServer.EventProcessing;
using Shuttle.Recall.SqlServer.Storage;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;
using Color = Terminal.Gui.Color;

namespace Orders;

internal class Program
{
    private static readonly List<LogEntry> LogEntries = [];

    private static ListView _outputListView = null!;

    private record LogEntry(string Message, Color Foreground)
    {
        public override string ToString() => Message;
    }

    private class Command
    {
        public string Key { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Color Color { get; init; }

        public override string ToString() => Description;
    }

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var host = new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(configuration)
                    .AddSqlServerEventStorage(builder =>
                    {
                        builder.Options.ConnectionString = configuration.GetConnectionString("StorageConnection") ?? throw new ApplicationException("A 'ConnectionString' with name 'StorageConnection' is required which points to a Sql Server database that will contain the event storage.");
                        builder.Options.Schema = "RecallSamples";
                    })
                    .AddSqlServerEventProcessing(builder =>
                    {
                        builder.Options.ConnectionString = configuration.GetConnectionString("EventProcessingConnection") ?? throw new ApplicationException("A 'ConnectionString' with name 'EventProcessingConnection' is required which points to a Sql Server database that will contain the projections.");
                        builder.Options.Schema = "RecallSamples";
                    })
                    .AddEventStore(builder =>
                    {
                        builder.AddProjection("orders").AddEventHandler<OrderHandler>();
                    });

                services.AddDbContextFactory<OrderDbContext>(dbContextFactoryBuilder =>
                {
                    dbContextFactoryBuilder.UseSqlServer(configuration.GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Order' is required which points to a Sql Server database that will contain the orders."));
                });
            })
            .Build();

        await host.StartAsync();

        // The above has to be before this; esle there are synchronization context/blocking issues.
        Application.Init();

        var defaultScheme = new ColorScheme()
        {
            Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
            Focus = Application.Driver.MakeAttribute(Color.Black, Color.Gray),
            HotNormal = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black),
            HotFocus = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Gray)
        };

        var top = Application.Top;
        top.ColorScheme = defaultScheme;

        var promptWin = new Window("Message Prompts")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Percent(40),
            ColorScheme = defaultScheme
        };

        var outputWin = new Window("System Output (Press Ctrl+Q to Exit)")
        {
            X = 0,
            Y = Pos.Bottom(promptWin),
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = defaultScheme
        };

        var commands = new List<Command>
        {
            new() { Key = "create", Description = "Create an order", Color = Color.Brown },
            new() { Key = "exit", Description = "(exit)", Color = Color.Magenta }
        };

        var commandListView = new ListView(commands)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
            ColorScheme = defaultScheme
        };

        commandListView.RowRender += args =>
        {
            if (commandListView.SelectedItem == args.Row)
            {
                return;
            }

            args.RowAttribute = new Attribute(commands[args.Row].Color, Color.Black);
        };

        _outputListView = new(LogEntries)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = false,
            ColorScheme = defaultScheme
        };

        _outputListView.RowRender += args =>
        {
            args.RowAttribute = new Attribute(LogEntries[args.Row].Foreground, Color.Black);
        };

        promptWin.Add(commandListView);
        outputWin.Add(_outputListView);
        top.Add(promptWin, outputWin);

        var eventStore = host.Services.GetRequiredService<IEventStore>();
        var faker = new Bogus.Faker();

        commandListView.OpenSelectedItem += async (args) =>
        {
            var cmd = (Command)args.Value;

            if (cmd.Key == "exit")
            {
                Application.RequestStop();
                return;
            }

            Log($"Action: Executing {cmd.Key}...", cmd.Color);

            try
            {
                switch (cmd.Key)
                {
                    case "create":
                    {
                        var person = new Bogus.Person();
                        var order = new Order(Guid.NewGuid());
                        var stream = await eventStore.GetAsync(order.Id);

                        stream.Add(order.Register($"{person.FirstName} {person.LastName}"));
                        stream.Add(order.AddItem("item-1", 1, 100));
                        stream.Add(order.AddItem("item-2", 2, 200));
                        stream.Add(order.AddItem("item-3", 3, 300));

                        await eventStore.SaveAsync(stream);

                        Log($"[order/created] : id = '{order.Id}' / customer name = '{order.CustomerName}'", Color.BrightCyan);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Send Error: {ex.Message}", Color.Red);
            }
        };

        Application.Run();
        Application.Shutdown();

        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Shut down successfully.");
        Console.WriteLine("------------------------------------------");

        Environment.Exit(0);
    }

    private static void Log(string message, Color color)
    {
        Application.MainLoop.Invoke(() =>
        {
            LogEntries.Add(new($"[{DateTime.Now:HH:mm:ss}] {message}", color));
            _outputListView.SetSource(LogEntries.ToList());

            if (LogEntries.Count <= 0)
            {
                return;
            }

            _outputListView.SelectedItem = LogEntries.Count - 1;
            _outputListView.EnsureSelectedItemVisible();
        });
    }
}