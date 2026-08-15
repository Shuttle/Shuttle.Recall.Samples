using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Data;
using Orders.Messages.v1;
using Shuttle.Hopper;
using Shuttle.Hopper.AzureStorageQueues;
using Shuttle.Hopper.SqlServer.Queue;
using Shuttle.Recall;
using Shuttle.Recall.SqlServer.Storage;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Orders.Client;

internal class Program
{
    private static readonly ObservableCollection<LogEntry> LogEntries = [];

    private static IApplication _app = null!;
    private static ListView<LogEntry> _outputListView = null!;

    private static void ClearLog()
    {
        _app.Invoke(() => LogEntries.Clear());
    }

    private static void Log(string message, Color color)
    {
        _app.Invoke(() =>
        {
            LogEntries.Add(new($"[{DateTime.Now:HH:mm:ss}] {message}", color));

            if (LogEntries.Count <= 0)
            {
                return;
            }

            _outputListView.Index = LogEntries.Count - 1;
            _outputListView.EnsureSelectedItemVisible();
        });
    }

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        var host = new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(configuration)
                    .AddRecall()
                    .UseSqlServerEventStorage(options =>
                    {
                        options.ConnectionString = configuration.GetConnectionString("Storage")
                                                   ?? throw new ApplicationException("A 'ConnectionString' with name 'Storage' is required which points to a Sql Server database that will contain the event storage.");
                        options.Schema = "recall_samples";
                    })
                    .Services
                    .AddHopper(options =>
                    {
                        configuration.GetSection(HopperOptions.SectionName).Bind(options);
                    })
                    .UseAzureStorageQueues(builder =>
                    {
                        builder.Configure("recall-samples", options =>
                        {
                            options.ConnectionString = "UseDevelopmentStorage=true;";
                        });
                    })
                    .UseSqlServerQueue(builder =>
                    {
                        builder
                            .Configure("recall-samples", options =>
                            {
                                options.ConnectionString = configuration.GetConnectionString("Orders")
                                                           ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the event storage.");
                                options.Schema = "recall_samples";
                            })
                            .UseOutboxDbContext<OrderDbContext>();
                    })
                    .Services
                    .AddOrderData();
            })
            .Build();

        await host.StartAsync();

        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();

        // The above has to be before this; else there are synchronization context/blocking issues.
        using var app = Application.Create().Init();
        _app = app;

        var defaultScheme = new Scheme
        {
            Normal = new(Color.White, Color.Black),
            Focus = new(Color.Black, Color.Gray),
            HotNormal = new(Color.BrightCyan, Color.Black),
            HotFocus = new(Color.BrightCyan, Color.Gray)
        };

        var top = new Window
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        top.SetScheme(defaultScheme);

        var promptWin = new Window
        {
            Title = "Message Prompts",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Percent(40)
        };
        promptWin.SetScheme(defaultScheme);

        var outputWin = new Window
        {
            Title = "System Output (Press Ctrl+Q to Exit)",
            X = 0,
            Y = Pos.Bottom(promptWin),
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        outputWin.SetScheme(defaultScheme);

        var commands = new List<Command>
        {
            new() { Key = "create", Description = "Create an order", Color = Color.Yellow },
            new() { Key = "create-fail", Description = "Create an order (fail, to test outbox)", Color = Color.BrightYellow },
            new() { Key = "list-orders", Description = "List last 5 orders", Color = Color.Yellow },
            new() { Key = "clear", Description = "Clear log", Color = Color.Yellow },
            //new() { Key = "reset", Description = "Reset all data (DESTRUCTIVE - PLEASE BE SURE)", Color = Color.Yellow },
            new() { Key = "exit", Description = "(exit)", Color = Color.Magenta }
        };

        var commandListView = new ListView<Command>
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true
        };
        commandListView.SetScheme(defaultScheme);
        commandListView.SetSource(new(commands));

        commandListView.RowRender += (_, args) =>
        {
            if (commandListView.Index == args.Row)
            {
                return;
            }

            args.RowAttribute = new(commands[args.Row].Color, Color.Black);
        };

        _outputListView = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = false
        };
        _outputListView.SetScheme(defaultScheme);
        _outputListView.SetSource(LogEntries);

        _outputListView.RowRender += (_, args) =>
        {
            args.RowAttribute = new(LogEntries[args.Row].Foreground, Color.Black);
        };

        promptWin.Add(commandListView);
        outputWin.Add(_outputListView);
        top.Add(promptWin, outputWin);

        commandListView.Accepting += async (_, args) =>
        {
            var cmd = commandListView.SelectedItem;

            if (cmd is null)
            {
                return;
            }

            args.Handled = true;

            if (cmd.Key == "exit")
            {
                _app.RequestStop();
                return;
            }

            Log($"Action: Executing {cmd.Key}...", cmd.Color);

            try
            {
                switch (cmd.Key)
                {
                    case "create":
                    {
                        using var scope = scopeFactory.CreateScope();
                        var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                        await bus.SendAsync(new CreateOrder());

                        Log("'CreateOrder' message sent.", Color.BrightCyan);

                        break;
                    }
                    case "create-fail":
                    {
                        using var scope = scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                        var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                        try
                        {
                            using (dbContext.Database.BeginTransactionAsync())
                            {
                                await bus.SendAsync(new CreateOrder());

                                Log("'CreateOrder' message sent.  Will fail.", Color.BrightCyan);

                                await dbContext.SaveChangesAsync();

                                throw new ApplicationException("Should not send message");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message, Color.BrightRed);
                        }

                        break;
                    }
                    case "clear":
                    {
                        ClearLog();
                        break;
                    }
                    case "list-orders":
                    {
                        using var scope = scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                        var orders = await dbContext.Orders.Include(item => item.Items).AsNoTracking()
                            .OrderByDescending(item => item.DateRegistered)
                            .Take(5)
                            .ToListAsync();

                        foreach (var order in orders)
                        {
                            Log($"[order] : id = '{order.Id}' / date registered = '{order.DateRegistered:O}' / customer name = '{order.CustomerName}' / total = {order.Total:C}", Color.Gray);

                            foreach (var item in order.Items)
                            {
                                Log($"   [item] : product = '{item.Product}' / quantity = '{item.Quantity}' / cost = {item.Cost:C} / total = {item.Total:C}", Color.DarkGray);
                            }
                        }

                        break;
                    }
                    case "reset":
                    {
                        var confirm = new Dialog { Title = "⚠️  Dangerous Operation", Width = 70, Height = 12 };

                        var warningLabel = new Label { X = 1, Y = 1, Text = "This will DELETE ALL DATA." };
                        warningLabel.SetScheme(new() { Normal = new(Color.BrightRed, Color.Black) });

                        confirm.Add(
                            warningLabel,
                            new Label { X = 1, Y = 3, Text = "Are you absolutely sure?" }
                        );

                        var no = new Button { Text = "No" };
                        var yes = new Button { Text = "Yes, delete everything" };

                        no.Accepting += (_, _) => _app.RequestStop(confirm);

                        yes.Accepting += (_, _) =>
                        {
                            _app.RequestStop(confirm);

                            Log("Reset confirmed by user.", Color.BrightRed);

                            // reset logic
                        };

                        confirm.AddButton(no);
                        confirm.AddButton(yes);

                        _app.Run(confirm);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Send Error: {ex.Message}", Color.Red);
            }
        };

        app.Run(top);
        top.Dispose();

        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Shut down successfully.");
        Console.WriteLine("------------------------------------------");

        Environment.Exit(0);
    }

    private class Command
    {
        public Color Color { get; init; }
        public string Description { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;

        public override string ToString()
        {
            return Description;
        }
    }

    private record LogEntry(string Message, Color Foreground)
    {
        public override string ToString()
        {
            return Message;
        }
    }
}