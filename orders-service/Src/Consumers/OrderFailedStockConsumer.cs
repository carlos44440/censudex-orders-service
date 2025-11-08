using MassTransit;
using Microsoft.OpenApi.Extensions;
using orders_service.Src.DTOs;
using orders_service.Src.Interfaces;
using Shared.OrderFailedStockMessage;

namespace ConsumerApi.Consumers
{
    public class OrderFailedStockConsumer : IConsumer<OrderFailedStockMessage>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderFailedStockConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        
        public async Task Consume(ConsumeContext<OrderFailedStockMessage> context)
        {
            var message = context.Message;
            Console.WriteLine($"[{message.FailedAt:HH:mm:ss}] {message.OrderId} - {message.FailedProducts}: {message.Reason}");

            var requestCancelOrderDto = new RequestCancelOrderDto
            {
                OrderId = message.OrderId,
                CancellationReason = message.Reason,
                FailedProducts = message.FailedProducts
            };

            var userData = new UserDataDto
            {
                Id = Guid.Empty,
                Name = "System",
                Role = "Admin",
                EmailAddress = "admin@gmail.com"
            };

            await _orderRepository.CancelOrderAsync(requestCancelOrderDto, userData);
        }
    }
}