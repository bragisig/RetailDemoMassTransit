using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Messages.Events;
using Microsoft.Extensions.Logging;

namespace Billing
{
    public class OrderPlacedHandler :
        IConsumer<OrderPlaced>
    {
        private readonly ILogger<OrderPlacedHandler> logger;
        Random rnd = new Random();
        
        public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
        {
            this.logger = logger;
            logger.LogInformation("OrderPlacedHandler constructed");
        }
        
        public async Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Thread.Sleep(rnd.Next(0, 3000));
            logger.LogInformation($"Received OrderPlaced, OrderId = {context.Message.OrderId} - Charging credit card...");
            
            //Make the charging of the card fail randomly to test the retry mechanism
            if (rnd.Next(0, 8) == 0)
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
            
            logger.LogInformation($"Credit card successfully charged, order billed at {orderBilled.BillingDate}");

            await context.Publish(orderBilled);

            await Task.CompletedTask;
        }
    }
}