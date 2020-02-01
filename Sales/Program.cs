using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Logging;
using Serilog;

namespace Sales
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
            Console.Title = "Sales";

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
                
                config.ReceiveEndpoint("Sales", endpointConfigurator =>
                {
                    endpointConfigurator.Consumer<PlaceOrderHandler>(consumerConfig =>
                    {
                       
                        //consumer config
                    });
                });
                
            });

            await busControl.StartAsync().ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await busControl.StopAsync().ConfigureAwait(false);
        }
    }
}