using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Messages.Events;

namespace Shipping
{
    public class OrderPlacedHandler :
        IConsumer<OrderPlaced>
    {
        private static readonly ILog Log = Logger.Get<OrderPlacedHandler>();
        Random rnd = new Random();
        
        public async Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Thread.Sleep(rnd.Next(0, 2000));
            Log.Info($"Received OrderPlaced, OrderId = {context.Message.OrderId} - Ship now???");

            await Task.CompletedTask;
        }
    }
}