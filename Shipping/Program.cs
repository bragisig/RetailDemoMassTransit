using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sales;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Shipping
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain(args).GetAwaiter().GetResult();
        }

        static async Task AsyncMain(string[] args)
        {
            Console.Title = "Shipping";

            var isService = !(Debugger.IsAttached || ((IList) args).Contains("--console"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostBuilderContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddSaga<ShippingSaga>().InMemoryRepository();
                        cfg.AddBus(ConfigureBus);
                    });
                    
                    services.AddHostedService<ShippingHostedService>();
                })
                .ConfigureLogging((hostBuilderContext, logging) =>
                {
                    logging.AddSerilog(Log.Logger);
                    logging.AddConfiguration(hostBuilderContext.Configuration.GetSection("Logging"));
                });

            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();

            Log.CloseAndFlush();
        }

        private static IBusControl ConfigureBus(IRegistrationContext<IServiceProvider> registrationContext)
        {
            return Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host(new Uri("rabbitmq://localhost/RetailDemoMassTransit"), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
                
                config.ConfigureEndpoints(registrationContext);

            });
        }
    }
}