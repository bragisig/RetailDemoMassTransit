// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using MassTransit;
// using Messages.Events;
// using Microsoft.Extensions.Logging;
//
// namespace Shipping
// {
//     public class OrderPlacedHandler :
//         IConsumer<OrderPlaced>
//     {
//         private readonly ILogger<OrderPlacedHandler> logger;
//         Random rnd = new Random();
//         
//         public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
//         {
//             this.logger = logger;
//         }
//
//         public async Task Consume(ConsumeContext<OrderPlaced> context)
//         {
//             Thread.Sleep(rnd.Next(0, 2000));
//             logger.LogInformation($"Received OrderPlaced, OrderId = {context.Message.OrderId} - Ship now???");
//
//              await Task.CompletedTask;
//         }
//     }
// }