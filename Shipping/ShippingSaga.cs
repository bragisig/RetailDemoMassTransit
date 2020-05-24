using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Saga;
using Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shipping
{
    public class ShippingSaga : 
        ISaga, 
        InitiatedBy<OrderPlaced>,
        Orchestrates<OrderBilled>
    {
        Random rnd = new Random();

        public Guid CorrelationId { get; set; }

        public DateTime? OrderPlacedDate { get; set; }
        public DateTime? OrderBilledDate { get; set; }
        
        public ShippingSaga()
        {
        }

        public ShippingSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            var logger = context.GetPayload<IServiceProvider>().GetService<ILogger<ShippingSaga>>();
            
            OrderPlacedDate = context.Message.OrderDate;
            
            Thread.Sleep(rnd.Next(0, 2000));
            logger.LogInformation($"Received OrderPlaced, OrderId = {context.Message.OrderId}, order placed date = {context.Message.OrderDate}");
            ProcessOrder(logger);
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<OrderBilled> context)
        {
            var logger = context.GetPayload<IServiceProvider>().GetService<ILogger<ShippingSaga>>();
            
            OrderBilledDate = context.Message.BillingDate;
            
            Thread.Sleep(rnd.Next(0, 2000));
            logger.LogInformation($"Received OrderBilled, OrderId = {context.Message.OrderId}, billing date  = {context.Message.BillingDate}");
            ProcessOrder(logger);
            return Task.CompletedTask;
        }
        
        private void ProcessOrder(ILogger<ShippingSaga> logger)
        {
            if (OrderPlacedDate.HasValue && OrderBilledDate.HasValue)
            {
                logger.LogInformation($"Order ready for shipping, OrderId = {CorrelationId}");
            }
        }
    }
}