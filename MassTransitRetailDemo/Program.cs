using System;
using System.Threading.Tasks;
using MassTransit;
using Messages.Commands;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MassTransitRetailDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AsyncMain().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
        }
        
        static async Task AsyncMain()
        {
            Console.Title = "MassTransitRetailDemoUI";
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            
            var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host(new Uri("rabbitmq://localhost/RetailDemoMassTransit"), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
            });
            
            await busControl.StartAsync().ConfigureAwait(false);
            
            await RunLoop(busControl)
                .ConfigureAwait(false);

            await busControl.StopAsync().ConfigureAwait(false);
        }
        
        static async Task RunLoop(IBusControl bus)
        {
            while (true)
            {
                Console.Write("Press 'P' to place an order, or 'Q' to quit: ");
                var key = Console.ReadKey();
                Console.WriteLine();
                
                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new PlaceOrder
                        {
                            OrderId = Guid.NewGuid(),
                        };

                        // Send the command to the local endpoint
                        Log.Information($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        
                        var r = await bus.GetSendEndpoint(new Uri("exchange:PlaceOrderHandler"));
                        await r.Send(command);
                        break;

                    case ConsoleKey.Q:
                        Log.Information("Q was pressed, shutting down.");
                        return;

                    default:
                        Log.Information("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}