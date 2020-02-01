using System;
using MassTransit;

namespace Messages.Events
{
    public class OrderBilled : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime BillingDate { get; set; }
    }
}