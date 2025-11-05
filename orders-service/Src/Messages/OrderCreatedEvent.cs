using orders_service.Src.DTOs;

namespace Shared.OrderCreatedEvent
{
    public class OrderCreatedEvent
    {
        public OrderDto Order { get; set; } = new OrderDto();
        public DateTime SentAt { get; set; }
    }
}