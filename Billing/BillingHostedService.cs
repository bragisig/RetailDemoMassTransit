namespace Billing
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Hosting;
    
    public class BillingHostedService :
        IHostedService
    {
        readonly IBusControl bus;

        public BillingHostedService(IBusControl bus)
        {
            this.bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return bus.StopAsync(cancellationToken);
        }
    }
}