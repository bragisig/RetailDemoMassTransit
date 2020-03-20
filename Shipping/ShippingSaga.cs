using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Saga;
using Messages.Events;

namespace Shipping
{
    public class ShippingSaga : 
        ISaga, 
        InitiatedBy<OrderPlaced>,
        Orchestrates<OrderBilled>
    {
        private static readonly ILog Log = Logger.Get<ShippingSaga>();
        Random rnd = new Random();

        public Guid CorrelationId { get; set; }

        public DateTime? OrderPlacedDate { get; set; }
        public DateTime? OrderBilledDate { get; set; }
        
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            OrderPlacedDate = context.Message.OrderDate;
            
            Thread.Sleep(rnd.Next(0, 2000));
            Log.Info($"Received OrderPlaced, OrderId = {context.Message.OrderId}, order placed date = {context.Message.OrderDate}");
            ProcessOrder();
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<OrderBilled> context)
        {
            OrderBilledDate = context.Message.BillingDate;
            
            Thread.Sleep(rnd.Next(0, 2000));
            Log.Info($"Received OrderBilled, OrderId = {context.Message.OrderId}, billing date  = {context.Message.BillingDate}");
            ProcessOrder();
            return Task.CompletedTask;
        }
        
        private void ProcessOrder()
        {
            if (OrderPlacedDate.HasValue && OrderBilledDate.HasValue)
            {
                Log.Info($"Order ready for shipping, OrderId = {CorrelationId}");
            }
        }
    }
}