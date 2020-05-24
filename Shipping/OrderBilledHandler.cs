// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using MassTransit;
// using Messages.Events;
// using Microsoft.Extensions.Logging;
//
// namespace Shipping
// {
//     public class OrderBilledHandler :
//         IConsumer<OrderBilled>
//     {
//         private readonly ILogger<OrderBilledHandler> logger;
//         Random rnd = new Random();
//         
//         public OrderBilledHandler(ILogger<OrderBilledHandler> logger)
//         {
//             this.logger = logger;
//         }
//
//         public async Task Consume(ConsumeContext<OrderBilled> context)
//         {
//             Thread.Sleep(rnd.Next(0, 2000));
//             logger.LogInformation($"Received OrderBilled, OrderId = {context.Message.OrderId} - Ship now???");
//
//             await Task.CompletedTask;
//         }
//     }
// }