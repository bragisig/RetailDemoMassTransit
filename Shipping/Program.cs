using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Saga;
using Serilog;

namespace Shipping
{
    class Program
    {
        private static readonly ILog Log = Logger.Get<Program>();

        static async Task Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "Shipping";

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
                
                var repository = new InMemorySagaRepository<ShippingSaga>();

                config.ReceiveEndpoint(endpointConfigurator =>
                {
                    // endpointConfigurator.Consumer<OrderPlacedHandler>(consumerConfig =>
                    // {
                    //     //consumer config
                    // });
                    // endpointConfigurator.Consumer<OrderBilledHandler>(consumerConfig =>
                    // {
                    //     //consumer config
                    // });
                    endpointConfigurator.Saga<ShippingSaga>(repository);
                });
                
            });

            await busControl.StartAsync().ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await busControl.StopAsync().ConfigureAwait(false);
        }
    }
}