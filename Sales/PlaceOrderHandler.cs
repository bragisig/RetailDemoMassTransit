using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Messages;
using Messages.Commands;
using Messages.Events;

namespace Sales
{
    public class PlaceOrderHandler :
        IConsumer<PlaceOrder>
    {
        private static readonly ILog Log = Logger.Get<PlaceOrderHandler>();
        Random rnd = new Random();
        
        public async Task Consume(ConsumeContext<PlaceOrder> context)
        {
            Thread.Sleep(rnd.Next(0, 2000));

            var orderPlaced = new OrderPlaced
            {
                OrderId = context.Message.OrderId,
                CorrelationId = context.Message.OrderId,
                OrderDate = DateTime.Now
            };
            
            Log.Info($"PlaceOrder -> Order processed, OrderId: {context.Message.OrderId}, order date: {orderPlaced.OrderDate}");

            await context.Publish(orderPlaced);

            await Task.CompletedTask;
        }
    }
}