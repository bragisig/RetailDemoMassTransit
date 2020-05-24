using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Messages.Commands;
using Messages.Events;
using Microsoft.Extensions.Logging;

namespace Sales
{
    public class PlaceOrderHandler :
        IConsumer<PlaceOrder>
    {
        private readonly ILogger<PlaceOrderHandler> logger;

        Random rnd = new Random();
        
        public PlaceOrderHandler(ILogger<PlaceOrderHandler> logger)
        {
            this.logger = logger;
            logger.LogInformation("PlaceOrderHandler constructed");
        }
        
        public async Task Consume(ConsumeContext<PlaceOrder> context)
        {
            Thread.Sleep(rnd.Next(0, 2000));

            var orderPlaced = new OrderPlaced
            {
                OrderId = context.Message.OrderId,
                CorrelationId = context.Message.OrderId,
                OrderDate = DateTime.Now
            };
            
            logger.LogInformation($"PlaceOrder -> Order processed, OrderId: {context.Message.OrderId}, order date: {orderPlaced.OrderDate}");

            await context.Publish(orderPlaced);

            await Task.CompletedTask;
        }
    }
}