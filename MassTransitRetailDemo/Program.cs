using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Messages;
using Messages.Commands;
using Serilog;

namespace MassTransitRetailDemo
{
    class Program
    {
        private static readonly ILog Log = Logger.Get<Program>();
        
        static async Task Main(string[] args)
        {
            try
            {
                AsyncMain().GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        
        static async Task AsyncMain()
        {
            Console.Title = "MassTransitRetailDemoUI";
            
            var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                config.UseSerilog(logger);

                config.Host(new Uri("rabbitmq://localhost/RetailDemoMassTransit"), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
            });
            
            EndpointConvention.Map<PlaceOrder>(new Uri("rabbitmq://localhost/RetailDemoMassTransit/Sales"));

            await busControl.StartAsync().ConfigureAwait(false);
            
            await RunLoop(busControl)
                .ConfigureAwait(false);

            await busControl.StopAsync().ConfigureAwait(false);
        }
        
        static async Task RunLoop(IBusControl bus)
        {
            while (true)
            {
                Log.Info("Press 'P' to place an order, or 'Q' to quit.");
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
                        Log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await bus.Send(command).ConfigureAwait(false);

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        Log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}