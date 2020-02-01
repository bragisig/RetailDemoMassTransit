using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Messages.Events;

namespace Shipping
{
    public class OrderBilledHandler :
        IConsumer<OrderBilled>
    {
        private static readonly ILog Log = Logger.Get<OrderBilledHandler>();
        Random rnd = new Random();
        
        public async Task Consume(ConsumeContext<OrderBilled> context)
        {
            Thread.Sleep(rnd.Next(0, 2000));
            Log.Info($"Received OrderBilled, OrderId = {context.Message.OrderId} - Ship now???");

            await Task.CompletedTask;
        }
    }
}