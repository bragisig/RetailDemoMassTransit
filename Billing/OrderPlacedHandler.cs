using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Messages.Events;

namespace Billing
{
    public class OrderPlacedHandler :
        IConsumer<OrderPlaced>
    {
        private static readonly ILog Log = Logger.Get<OrderPlacedHandler>();
        Random rnd = new Random();

        public async Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Thread.Sleep(rnd.Next(0, 3000));
            Log.Info($"Received OrderPlaced, OrderId = {context.Message.OrderId} - Charging credit card...");
            
            //Make the charging of the card fail randomly to test the retry mechanism
            if (rnd.Next(0, 3) == 0)
            {
                throw new Exception("Credit card charging failed with an exception!");
            }
            
            Thread.Sleep(rnd.Next(2000, 5000));
            
            
            var orderBilled = new OrderBilled
            {
                OrderId = context.Message.OrderId,
                CorrelationId = context.Message.OrderId,
                BillingDate = DateTime.Now
            };
            
            Log.Info($"Credit card successfully charged, order billed at {orderBilled.BillingDate}");

            await context.Publish(orderBilled);

            await Task.CompletedTask;
        }
    }
}