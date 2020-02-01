using System;
using MassTransit;

namespace Messages.Events
{
    public class OrderPlaced : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}