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
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;
using Color = Terminal.Gui.Color;

namespace Orders.Client;

internal class Program
{
    private static readonly List<LogEntry> LogEntries = [];

    private static ListView _outputListView = null!;

    private static void ClearLog()
    {
        Application.MainLoop.Invoke(() =>
        {
            LogEntries.Clear();
            _outputListView.SetSource(LogEntries.ToList());
        });
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

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        var host = new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                var connectionString = configuration.GetConnectionString("Storage")
                                       ?? throw new ApplicationException("A 'ConnectionString' with name 'Storage' is required which points to a Sql Server database that will contain the event storage.");

                services.AddSingleton<IConfiguration>(configuration)
                    .AddRecall()
                    .UseSqlServerEventStorage(options =>
                    {
                        options.ConnectionString = connectionString;
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
                        builder.Configure("recall-samples", options =>
                        {
                            options.ConnectionString = connectionString;
                            options.Schema = "recall_samples";

                            options.WithOutboxDbContext<OrderDbContext>();
                        });
                    })
                    .Services
                    .AddOrderData();
            })
            .Build();

        await host.StartAsync();

        var bus = host.Services.GetRequiredService<IBus>();
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();

        // The above has to be before this; else there are synchronization context/blocking issues.
        Application.Init();

        var defaultScheme = new ColorScheme
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
            new() { Key = "create-fail", Description = "Create an order (fail, to test outbox)", Color = Color.BrightYellow },
            new() { Key = "list-orders", Description = "List last 5 orders", Color = Color.Brown },
            new() { Key = "clear", Description = "Clear log", Color = Color.Brown },
            //new() { Key = "reset", Description = "Reset all data (DESTRUCTIVE - PLEASE BE SURE)", Color = Color.Brown },
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

        commandListView.OpenSelectedItem += async args =>
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
                        await bus.SendAsync(new CreateOrder());

                        Log("'CreateOrder' message sent.", Color.BrightCyan);

                        break;
                    }
                    case "create-fail":
                    {
                        using var scope = scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

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
                        var confirm = new Dialog("⚠️  Dangerous Operation", 70, 12);

                        confirm.Add(
                            new Label(1, 1, "This will DELETE ALL DATA.")
                            {
                                ColorScheme = new()
                                {
                                    Normal = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black)
                                }
                            },
                            new Label(1, 3, "Are you absolutely sure?")
                        );

                        var no = new Button("No");
                        var yes = new Button("Yes, delete everything");

                        no.Clicked += () => Application.RequestStop();

                        yes.Clicked += () =>
                        {
                            Application.RequestStop();

                            Log("Reset confirmed by user.", Color.BrightRed);

                            // reset logic
                        };

                        confirm.AddButton(no);
                        confirm.AddButton(yes);

                        Application.Run(confirm);
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