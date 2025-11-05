using MassTransit;
using orders_service.Src.DTOs;
using orders_service.Src.Interfaces;
using Shared.OrderFailedStockEvent;

namespace ConsumerApi.Consumers
{
    public class OrderFailedStockConsumer : IConsumer<OrderFailedStockEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderFailedStockConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        
        public async Task Consume(ConsumeContext<OrderFailedStockEvent> context)
        {
            var message = context.Message;
            Console.WriteLine($"[{message.FailedAt:HH:mm:ss}] {message.OrderId}: {message.Reason}");

            var requestCancelOrderDto = new RequestCancelOrderDto
            {
                OrderId = message.OrderId,
                CancellationReason = message.Reason,
                OutOfStockProductId = message.ProductId
            };

            var userData = new UserDataDto
            {
                Id = "12212sss",
                Name = "Admin",
                Role = "Admin",
                EmailAddress = "admin@ucn.cl"
            };

            await _orderRepository.CancelOrderAsync(requestCancelOrderDto, userData);
        }
    }
}