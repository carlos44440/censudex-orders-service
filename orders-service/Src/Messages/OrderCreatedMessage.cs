using orders_service.Src.Messages;

namespace Shared.OrderCreatedMessage
{
    public class OrderCreatedMessage
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderItemMessage> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}